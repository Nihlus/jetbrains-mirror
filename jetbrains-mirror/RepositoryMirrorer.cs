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
using System.Collections.Generic;
using System.IO;
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
    public class RepositoryMirrorer
    {
        private readonly JetbrainsPlugins _api;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryMirrorer"/> class.
        /// </summary>
        public RepositoryMirrorer()
        {
            _api = new JetbrainsPlugins();
        }

        /// <summary>
        /// Mirrors the given repository, queueing its plugins up for download.
        /// </summary>
        /// <param name="repository">The repository to mirror.</param>
        /// <param name="ct">The cancellation token in use.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task MirrorRepositoryAsync(PluginRepository repository, CancellationToken ct)
        {
            var finalizedDownloads = new List<Task>();

            var baseDirectory = Path.Combine(Program.Options.OutputFolder, "plugins");

            if (Program.Options.VerboseOutput)
            {
                await Console.Out.WriteLineAsync("Creating directory tree...");
            }

            Directory.CreateDirectory(baseDirectory);

            foreach (var category in repository.Categories)
            {
                Directory.CreateDirectory(Path.Combine(baseDirectory, category.Name.GenerateSlug()));
            }

            foreach (var category in repository.Categories)
            {
                var targetDirectory = Path.Combine(baseDirectory, category.Name.GenerateSlug());

                if (Program.Options.VerboseOutput)
                {
                    await Console.Out.WriteLineAsync
                    (
                        $"Spinning up {category.Plugins.Count} downloads from \"{category.Name}\"..."
                    );
                }

                foreach (var plugin in category.Plugins)
                {
                    var downloadTask = FinalizeDownload(DownloadPluginAsync(targetDirectory, plugin, ct));
                    finalizedDownloads.Add(downloadTask);
                }
            }

            await Task.WhenAll(finalizedDownloads);
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

                    if (data.Content?.Headers?.ContentDisposition?.FileName is null)
                    {
                        return DownloadResult.FromError
                        (
                            plugin,
                            DownloadError.Unknown,
                            "Failed to retrieve file information from the download headers."
                        );
                    }

                    var sluggedPluginName = plugin.Name.GenerateSlug();
                    var filename = data.Content.Headers.ContentDisposition.FileName;
                    var version = plugin.Version;

                    var saveDirectory = Path.Combine
                    (
                        targetDirectory,
                        sluggedPluginName,
                        version
                    );

                    Directory.CreateDirectory(saveDirectory);

                    var savePath = Path.Combine(saveDirectory, filename.Replace("\"", string.Empty));

                    if (File.Exists(savePath))
                    {
                        if ((ulong)new FileInfo(savePath).Length == plugin.Size)
                        {
                            // Looks like we already have this one
                            return DownloadResult.FromSuccess(plugin, DownloadAction.Skipped);
                        }

                        // It's crap, so delete it and download again
                        File.Delete(savePath);
                    }

                    // Download to a temporary file first
                    var tempFile = Path.GetTempFileName();
                    using (var tempOutput = File.Create(tempFile))
                    {
                        using (var contentStream = await data.Content.ReadAsStreamAsync())
                        {
                            await contentStream.CopyToAsync(tempOutput, ct);
                        }
                    }

                    // And then move it over to the final save location
                    File.Move(tempFile, savePath);

                    return DownloadResult.FromSuccess(plugin, DownloadAction.Downloaded);
                }
            }
            catch (OperationCanceledException oex)
            {
                return DownloadResult.FromError(plugin, DownloadError.Timeout, oex.Message);
            }
            catch (Exception ex)
            {
                return DownloadResult.FromError(plugin, DownloadError.Exception, ex.Message, ex);
            }
        }

        private static async Task FinalizeDownload(Task<DownloadResult> downloadTask)
        {
            var result = await downloadTask;
            var plugin = result.Plugin;

            var pluginName = "Unknown";
            if (!(plugin is null))
            {
                pluginName = plugin.Name;
            }

            if (!result.IsSuccess)
            {
                switch (result.Error)
                {
                    case DownloadError.Exception:
                    {
                        await Console.Error.WriteLineAsync
                        (
                            $"[{nameof(RepositoryMirrorer)}]: Failed to download {pluginName} due to an exception: " +
                            $"{result.Exception?.Message ?? string.Empty}"
                        );

                        break;
                    }

                    case DownloadError.Timeout:
                    {
                        await Console.Error.WriteLineAsync
                        (
                            $"[{nameof(RepositoryMirrorer)}]: Failed to download {pluginName}: The download timed out."
                        );

                        break;
                    }

                    case DownloadError.Unknown:
                    case DownloadError.InvalidResponse:
                    {
                        await Console.Error.WriteLineAsync
                        (
                            $"[{nameof(RepositoryMirrorer)}]: Failed to download {pluginName}: {result.ErrorReason}"
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

                return;
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
                    if (Program.Options.VerboseOutput)
                    {
                        await Console.Out.WriteLineAsync
                        (
                            $"[{nameof(RepositoryMirrorer)}]: {pluginName} already exists; skipped download."
                        );
                    }

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
