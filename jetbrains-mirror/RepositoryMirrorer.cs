//
//  RepositoryMirrorer.cs
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
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Humanizer.Bytes;
using JetBrains.Mirror.API;
using JetBrains.Mirror.Helpers;
using JetBrains.Mirror.Results;
using JetBrains.Mirror.XML;

namespace JetBrains.Mirror
{
    /// <summary>
    /// Handles mirroring a repository.
    /// </summary>
    public class RepositoryMirrorer : IDisposable
    {
        private readonly JetbrainsPlugins _api;
        private readonly CancellationTokenSource _downloadCanceller;

        private readonly Task _downloadLoop;

        private readonly ConcurrentQueue<Task<DownloadResult>> _downloadQueue;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryMirrorer"/> class.
        /// </summary>
        public RepositoryMirrorer()
        {
            _api = new JetbrainsPlugins();
            _downloadCanceller = new CancellationTokenSource();

            _downloadQueue = new ConcurrentQueue<Task<DownloadResult>>();
            _downloadLoop = Task.Factory.StartNew(() => DownloadLoopAsync(_downloadCanceller.Token));
        }

        /// <summary>
        /// Mirrors the given repository, queueing its plugins up for download.
        /// </summary>
        /// <param name="repository">The repository to mirror.</param>
        /// <param name="ct">The cancellation token in use.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task MirrorRepositoryAsync(PluginRepository repository, CancellationToken ct)
        {
            const string baseDirectory = "plugins";

            foreach (var category in repository.Categories)
            {
                var targetDirectory = Path.Combine(baseDirectory, category.Name.GenerateSlug());

                foreach (var plugin in category.Plugins)
                {
                    var downloadTask = DownloadPluginAsync(targetDirectory, plugin, ct);
                    _downloadQueue.Enqueue(downloadTask);
                }

                Console.WriteLine($"Spun up {category.Plugins.Count} downloads from \"{category.Name}\"...");
            }

            while (!_downloadQueue.IsEmpty)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), ct);
            }
        }

        /// <summary>
        /// Downloads the given plugin.
        /// </summary>
        /// <param name="targetDirectory">The directory in which to save the plugin.</param>
        /// <param name="plugin">The plugin to download.</param>
        /// <param name="ct">The cancellation token in use.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task<DownloadResult> DownloadPluginAsync
        (
            string targetDirectory,
            IdeaPlugin plugin,
            CancellationToken ct
        )
        {
            try
            {
                using (var data = await _api.DownloadAsync(plugin, ct))
                {
                    if (!data.IsSuccessStatusCode)
                    {
                        return DownloadResult.FromError(plugin, DownloadError.Unknown, data.ReasonPhrase);
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
                            return DownloadResult.FromSuccess(plugin, DownloadAction.Skipped);
                        }
                    }

                    await File.WriteAllBytesAsync(savePath, await data.Content.ReadAsByteArrayAsync(), ct);
                    return DownloadResult.FromSuccess(plugin, DownloadAction.Downloaded);
                }
            }
            catch (OperationCanceledException oex)
            {
                return DownloadResult.FromError(plugin, DownloadError.Timeout, oex.Message);
            }
            catch (IOException iex)
            {
                await Console.Error.WriteLineAsync($"Download of {plugin.Name} failed: {iex.Message}");
                return DownloadResult.FromError(plugin, DownloadError.Exception, iex.Message);
            }
        }

        /// <summary>
        /// Continually runs until the mirrorer is disposed, consuming running download tasks.
        /// </summary>
        /// <param name="ct">The cancellation token in use.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task DownloadLoopAsync(CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
            {
                return;
            }

            while (!ct.IsCancellationRequested)
            {
                while (!_downloadQueue.IsEmpty)
                {
                    if (!_downloadQueue.TryDequeue(out var downloadTask))
                    {
                        continue;
                    }

                    if (!downloadTask.IsCompleted)
                    {
                        _downloadQueue.Enqueue(downloadTask);
                        continue;
                    }

                    var result = await downloadTask;
                    var plugin = result.Plugin;

                    var pluginName = "Unknown";
                    if (!(plugin is null))
                    {
                        pluginName = plugin.Name;
                    }

                    if (!result.IsSuccess)
                    {
                        await Console.Error.WriteLineAsync
                        (
                            $"[{nameof(RepositoryMirrorer)}]: Failed to download {pluginName}. {result.ErrorReason}"
                        );

                        continue;
                    }

                    switch (result.Action)
                    {
                        case DownloadAction.Downloaded:
                        {
                            var printableSize = string.Empty;
                            if (!(plugin is null))
                            {
                                var size = new ByteSize(plugin.Size);
                                printableSize = $"{size.LargestWholeNumberValue:F1} {size.LargestWholeNumberSymbol}";
                            }

                            await Console.Out.WriteLineAsync
                            (
                                $"[{nameof(RepositoryMirrorer)}]: Downloaded {pluginName} ({printableSize})"
                            );

                            break;
                        }

                        case DownloadAction.Skipped:
                        {
                            await Console.Out.WriteLineAsync
                            (
                                $"[{nameof(RepositoryMirrorer)}]: {pluginName} already exists; skipped download."
                            );
                            break;
                        }

                        case null:
                        {
                            break;
                        }

                        default:
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }
        }

        /// <inheritdoc/>
        public async void Dispose()
        {
            _downloadCanceller?.Dispose();

            // TODO: Use async streams
            await _downloadLoop;
        }
    }
}
