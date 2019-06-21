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
using Humanizer.Bytes;
using JetBrains.Mirror.API;

namespace JetBrains.Mirror
{
    /// <summary>
    /// Represents the main class of the program.
    /// </summary>
    internal static class Program
    {
        private static async Task Main()
        {
            var api = new JetbrainsPlugins();

            var targetBuilds = new[]
            {
                "RD-191.7141.355"
            };

            var mirrorer = new RepositoryMirrorer();
            using (var cancellationSource = new CancellationTokenSource())
            {
                foreach (var targetBuild in targetBuilds)
                {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();

                    await Console.Out.WriteLineAsync($"Fetching latest plugin versions for {targetBuild}...");

                    var repository = await api.ListPluginsAsync(targetBuilds[0], cancellationSource.Token);
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
