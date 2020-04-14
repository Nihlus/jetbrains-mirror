//
//  Program.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) 2019 Jarl Gullberg
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Affero General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Affero General Public License for more details.
//
//  You should have received a copy of the GNU Affero General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CommandLine;
using JetBrains.Plugins.Import.Extensions;
using JetBrains.Plugins.Models;
using JetBrains.Plugins.Models.API.XML;
using JetBrains.Plugins.Models.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MoreLinq;

namespace JetBrains.Plugins.Import
{
    /// <summary>
    /// Represents the main class of the program.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Gets the command-line options that were passed to the program.
        /// </summary>
        public static ProgramOptions Options { get; private set; } = null!;

        private static async Task<int> Main(string[] args)
        {
            var parsingResult = Parser.Default.ParseArguments<ProgramOptions>(args);
            if (parsingResult is Parsed<ProgramOptions> success)
            {
                Options = success.Value;
            }
            else
            {
                return 1;
            }

            if (!Directory.Exists(Options.InputFolder))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                await Console.Error.WriteLineAsync("Input directory not found.");

                return 1;
            }

            var repoFilePath = Path.Combine(Options.InputFolder, "plugins", "repository.xml");
            if (!File.Exists(repoFilePath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                await Console.Error.WriteLineAsync("No repository information found in the input folder.");

                return 1;
            }

            if (!File.Exists(Options.AuthenticationFile))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                await Console.Error.WriteLineAsync("Authentication file not found.");

                return 1;
            }

            if (!Passfile.TryParse(File.ReadAllText(Options.AuthenticationFile), out var passfile))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                await Console.Error.WriteLineAsync("Could not parse authentication file.");

                return 1;
            }

            var deserializer = new XmlSerializer(typeof(IdeaPluginRepository));

            IdeaPluginRepository repository;
            await using (var repoFile = File.OpenRead(repoFilePath))
            {
                repository = (IdeaPluginRepository)deserializer.Deserialize(repoFile);
            }

            await using (var services = new ServiceCollection()
                .AddDbContextPool<PluginsDatabaseContext>
                (
                    options => PluginsDatabaseContext
                        .ConfigureDefaultOptions(options)
                        .UseNpgsql(passfile.ConnectionString)
                )
                .BuildServiceProvider())
            {
                await ImportRepositoryAsync(services, repository);
            }

            return 0;
        }

        private static async Task ImportRepositoryAsync(IServiceProvider services, IdeaPluginRepository repository)
        {
            async Task ImportPluginReleaseScoped(IdeaPlugin pluginRelease)
            {
                using var scope = services.CreateScope();
                await using var db = scope.ServiceProvider.GetRequiredService<PluginsDatabaseContext>();

                if (await ImportPluginReleaseAsync(db, pluginRelease))
                {
                    await db.SaveChangesAsync();
                }
            }

            async Task ImportPluginScopedAsync(IdeaPlugin pluginDefinition, IdeaPluginCategory category)
            {
                using var scope = services.CreateScope();
                await using var db = scope.ServiceProvider.GetRequiredService<PluginsDatabaseContext>();
                if (await ImportPluginAsync(db, pluginDefinition, category))
                {
                    await db.SaveChangesAsync();
                }
            }

            async Task ImportCategoryScopedAsync(IdeaPluginCategory category)
            {
                using var scope = services.CreateScope();
                await Console.Out.WriteLineAsync($"Importing category \"{category.Name}\"...");

                await using var db = scope.ServiceProvider.GetRequiredService<PluginsDatabaseContext>();
                if (await ImportCategoryAsync(db, category))
                {
                    await db.SaveChangesAsync();
                }
            }

            // Stage 1: Import categories
            await Console.Out.WriteLineAsync("Importing categories and plugin definitions...");
            await Task.WhenAll(repository.Categories.Select(ImportCategoryScopedAsync));

            var importDefinitionTasks = new List<Task>();

            // Stage 2: Import plugin definitions
            foreach (var category in repository.Categories)
            {
                var pluginDefinitions = category.Plugins.GroupBy(p => p.ID).Select(g => g.First()).ToList();

                await Console.Out.WriteLineAsync
                (
                    $"Importing {pluginDefinitions.Count} plugins from \"{category.Name}\"..."
                );

                importDefinitionTasks.AddRange(pluginDefinitions.Select(p => ImportPluginScopedAsync(p, category)));
            }

            await Task.WhenAll(importDefinitionTasks);

            await Console.Out.WriteLineAsync("Importing releases...");

            var allReleases = repository.Categories.SelectMany(c => c.Plugins).ToList();

            // Stage 3: Import plugin releases
            var remaining = allReleases.Count;
            foreach (var plugin in allReleases.Batch(16))
            {
                var enumeratedBatch = plugin.ToList();

                var importReleaseTasks = enumeratedBatch.Select(ImportPluginReleaseScoped);
                await Task.WhenAll(importReleaseTasks);

                remaining -= enumeratedBatch.Count;
                await Console.Out.WriteLineAsync
                (
                    $"Imported batch ({enumeratedBatch.Count}) - {remaining} releases remaining."
                );
            }
        }

        private static async Task<bool> ImportPluginReleaseAsync
        (
            PluginsDatabaseContext db,
            IdeaPlugin pluginRelease
        )
        {
            var dbPlugin = await db.Plugins.FirstOrDefaultAsync(p => p.PluginID == pluginRelease.ID);

            var dbRelease = dbPlugin.Releases.FirstOrDefault(r => r.Version == pluginRelease.Version);
            if (dbRelease is null)
            {
                dbRelease = pluginRelease.ToReleaseEntity(dbPlugin);
                dbPlugin.Releases.Add(dbRelease);
            }
            else
            {
                var newValues = pluginRelease.ToReleaseEntity(dbPlugin);
                newValues.ID = dbRelease.ID;

                var existingEntry = db.Entry(dbRelease);
                existingEntry.CurrentValues.SetValues(newValues);

                if (existingEntry.State == EntityState.Unchanged)
                {
                    return false;
                }
            }

            return true;
        }

        private static async Task<bool> ImportPluginAsync
        (
            PluginsDatabaseContext db,
            IdeaPlugin plugin,
            IdeaPluginCategory category
        )
        {
            var dbCategory = await db.Categories.FirstOrDefaultAsync(c => c.Name == category.Name);
            if (dbCategory is null)
            {
                return false;
            }

            var dbPlugin = await db.Plugins.FirstOrDefaultAsync(p => p.PluginID == plugin.ID);
            if (dbPlugin is null)
            {
                dbPlugin = plugin.ToEntity(dbCategory);
                db.Plugins.Add(dbPlugin);
            }
            else
            {
                var newValues = plugin.ToEntity(dbCategory);
                newValues.ID = dbPlugin.ID;

                var existingEntry = db.Entry(dbPlugin);
                existingEntry.CurrentValues.SetValues(newValues);

                if (existingEntry.State == EntityState.Unchanged)
                {
                    return false;
                }
            }

            return true;
        }

        private static async Task<bool> ImportCategoryAsync
        (
            PluginsDatabaseContext db,
            IdeaPluginCategory category
        )
        {
            var dbCategory = await db.Categories.FirstOrDefaultAsync(c => c.Name == category.Name);
            if (dbCategory is null)
            {
                dbCategory = category.ToEntity();
                db.Categories.Add(dbCategory);
            }
            else
            {
                var newValues = category.ToEntity();
                newValues.ID = dbCategory.ID;

                var existingEntry = db.Entry(dbCategory);
                existingEntry.CurrentValues.SetValues(newValues);

                if (existingEntry.State == EntityState.Unchanged)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
