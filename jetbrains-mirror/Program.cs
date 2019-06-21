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
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Humanizer.Bytes;
using JetBrains.Mirror.API;

namespace JetBrains.Mirror
{
    /// <summary>
    /// Represents the main class of the program.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Gets the command-line options that were passed to the program.
        /// </summary>
        public static ProgramOptions Options { get; private set; }

        private static async Task Main(string[] args)
        {
            var parsingResult = Parser.Default.ParseArguments<ProgramOptions>(args);
            if (parsingResult is Parsed<ProgramOptions> success)
            {
                Options = success.Value;
            }
            else
            {
                return;
            }

            var api = new JetbrainsPlugins();

            var mirrorer = new RepositoryMirrorer();
            using (var cancellationSource = new CancellationTokenSource())
            {
                foreach (var productVersion in Options.ProductVersions)
                {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();

                    await Console.Out.WriteLineAsync($"Fetching latest plugin versions for {productVersion}...");

                    var repository = await api.ListPluginsAsync(productVersion, cancellationSource.Token);
                    var totalSize = new ByteSize(repository.Categories.SelectMany(p => p.Plugins).Select(p => p.Size).Aggregate((a, b) => a + b));

                    await Console.Out.WriteLineAsync
                    (
                        $"Done. Estimated total download size: " +
                        $"{totalSize.LargestWholeNumberValue:F1} {totalSize.LargestWholeNumberSymbol}\n"
                    );

                    await Console.Out.WriteLineAsync("Done. Starting mirroring...");
                    await mirrorer.MirrorRepositoryAsync(repository, cancellationSource.Token);
                }
            }
        }
    }
}
