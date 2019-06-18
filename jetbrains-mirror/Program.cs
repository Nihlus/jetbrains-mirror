//
//  Program.cs
//
//  Copyright (c) 2019 Firwood Software
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Humanizer;
using Humanizer.Bytes;
using JetBrains.Mirror.API;
using JetBrains.Mirror.Helpers;
using JetBrains.Mirror.XML;

namespace JetBrains.Mirror
{
    /// <summary>
    /// Represents the main class of the program.
    /// </summary>
    internal static class Program
    {
        private const string JetbrainsBaseUrl = "plugins.jetbrains.com";

        private static async Task Main(string[] args)
        {
            var api = new JetbrainsPlugins();
            async Task<(bool, IdeaPlugin)> DownloadAsync(string targetDirectory, IdeaPlugin plugin)
            {
                var data = await api.DownloadAsync(plugin);

                if (!data.IsSuccessStatusCode)
                {
                    return (false, plugin);
                }

                var filename = data.Content.Headers?.ContentDisposition?.FileName;
                if (filename is null)
                {
                    filename = $"{plugin.Name}.zip";
                }

                var savePath = Path.Combine(targetDirectory, filename.Replace("\"", string.Empty));
                if (File.Exists(savePath))
                {
                    if ((ulong)new FileInfo(savePath).Length == plugin.Size)
                    {
                        // Looks like we already have this one
                        return (true, plugin);
                    }
                }

                await File.WriteAllBytesAsync(savePath, await data.Content.ReadAsByteArrayAsync());
                return (true, plugin);
            }

            var targetBuilds = new[]
            {
                "RD-191.7141.355"
            };

            foreach (var targetBuild in targetBuilds)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                Console.WriteLine($"Fetching latest plugin versions for {targetBuild}...");

                var riderPlugins = await api.ListPluginsAsync(targetBuilds[0]);
                var totalSize = new ByteSize(riderPlugins.Categories.SelectMany(p => p.Plugins).Select(p => p.Size).Aggregate((a, b) => a + b));

                Console.WriteLine
                (
                    $"Done. Estimated total download size: " +
                    $"{totalSize.LargestWholeNumberValue:F1} {totalSize.LargestWholeNumberSymbol}\n" +
                    $"\n" +
                    $"Creating directory structure..."
                );

                var baseDirectory = Path.Combine("plugins", targetBuild);
                Directory.CreateDirectory(baseDirectory);

                foreach (var category in riderPlugins.Categories)
                {
                    Directory.CreateDirectory(Path.Combine(baseDirectory, category.Name.GenerateSlug()));
                }

                Console.WriteLine("Done. Spinning up downloads...");

                var downloadTasks = new Queue<Task<(bool success, IdeaPlugin plugin)>>();
                foreach (var category in riderPlugins.Categories)
                {
                    Console.WriteLine($"Spun up {category.Plugins.Count} downloads from \"{category.Name}\"...");
                    var targetDirectory = Path.Combine(baseDirectory, category.Name.GenerateSlug());

                    foreach (var downloadTask in category.Plugins.Select(p => DownloadAsync(targetDirectory, p)))
                    {
                        //downloadTasks.Enqueue(downloadTask);

                        try
                        {
                            var (success, plugin) = await downloadTask;
                            //++finishedDownloadCount;

                            if (!success)
                            {
                                Console.WriteLine($"Failed to download {plugin.Name}.");
                                continue;
                            }

                            var pluginSize = new ByteSize(plugin.Size);
                            Console.WriteLine
                            (
                                $"Finished downloading {plugin.Name} " +
                                $"(~{pluginSize.LargestWholeNumberValue:F1} {pluginSize.LargestWholeNumberSymbol})"// +
                                //$" - {finishedDownloadCount} of {totalDownloads}"
                            );
                        }
                        catch (TaskCanceledException tex)
                        {
                            Console.WriteLine("Download cancelled for whatever reason.");
                        }
                    }
                }

                return;

                ulong finishedDownloadCount = 0;
                var totalDownloads = downloadTasks.LongCount();

                while (downloadTasks.Count > 0)
                {
                    if (!downloadTasks.TryDequeue(out var downloadTask))
                    {
                        continue;
                    }

                    if (!downloadTask.IsCompleted)
                    {
                        // Requeue the task
                        downloadTasks.Enqueue(downloadTask);
                        continue;
                    }

                    var (success, plugin) = await downloadTask;
                    ++finishedDownloadCount;

                    if (!success)
                    {
                        Console.WriteLine($"Failed to download {plugin.Name}.");
                        continue;
                    }

                    var pluginSize = new ByteSize(plugin.Size);
                    Console.WriteLine
                    (
                        $"Finished downloading {plugin.Name} " +
                        $"(~{pluginSize.LargestWholeNumberValue:F1} {pluginSize.LargestWholeNumberSymbol})" +
                        $" - {finishedDownloadCount} of {totalDownloads}"
                    );
                }

                Console.WriteLine("Finished downloading ");
            }
        }
    }
}
