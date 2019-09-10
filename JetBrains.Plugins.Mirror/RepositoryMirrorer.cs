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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Humanizer.Bytes;
using JetBrains.Annotations;
using JetBrains.Plugins.Mirror.API;
using JetBrains.Plugins.Mirror.Helpers;
using JetBrains.Plugins.Mirror.Results;
using JetBrains.Plugins.Models.API.XML;

namespace JetBrains.Plugins.Mirror
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
        /// <param name="api">The API instance to use.</param>
        public RepositoryMirrorer(JetbrainsPlugins api)
        {
            _api = api;
        }

        /// <summary>
        /// Mirrors the given repository, queueing its plugins up for download.
        /// </summary>
        /// <param name="repository">The repository to mirror.</param>
        /// <param name="ct">The cancellation token in use.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task MirrorRepositoryAsync([NotNull] IdeaPluginRepository repository, CancellationToken ct)
        {
            var totalSize = new ByteSize
            (
                repository.Categories.SelectMany(p => p.Plugins).Select(p => p.Size).Aggregate((a, b) => a + b)
            );

            await Console.Out.WriteLineAsync
            (
                $"Estimated total download size: " +
                $"{totalSize.LargestWholeNumberValue:F1} {totalSize.LargestWholeNumberSymbol}\n"
            );

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

            await Console.Out.WriteLineAsync("Done. Starting mirroring...");

            var categoryDownloads = new List<(string categoryName, Task<DownloadResult[]> results)>();
            foreach (var category in repository.Categories)
            {
                var finalizedDownloads = new List<Task<DownloadResult>>();
                var targetDirectory = Path.Combine(baseDirectory, category.Name.GenerateSlug());

                if (Program.Options.VerboseOutput)
                {
                    await Console.Out.WriteLineAsync
                    (
                        $"Downloading {category.Plugins.Count} plugins from \"{category.Name}\"..."
                    );
                }

                foreach (var plugin in category.Plugins)
                {
                    if (Program.Options.MirrorAllVersions)
                    {
                        try
                        {
                            var pluginVersions = await _api.ListVersionsAsync(plugin.ID, ct);
                            foreach (var versionedPlugin in pluginVersions)
                            {
                                var downloadTask = FinalizeDownload
                                (
                                    DownloadPluginAsync(targetDirectory, versionedPlugin, ct)
                                );

                                finalizedDownloads.Add(downloadTask);
                            }
                        }
                        catch (TimeoutException)
                        {
                            await Console.Out.WriteLineAsync
                            (
                                $"[{nameof(RepositoryMirrorer)}]: Failed to fetch version information for " +
                                $"{plugin.Name}: The download timed out."
                            );
                        }
                    }
                    else
                    {
                        var downloadTask = FinalizeDownload(DownloadPluginAsync(targetDirectory, plugin, ct));
                        finalizedDownloads.Add(downloadTask);
                    }
                }

                categoryDownloads.Add((category.Name, Task.WhenAll(finalizedDownloads)));
            }

            // Finally, write the successfully mirrored repository data back out to disk
            var categoryResults = await Task.WhenAll(categoryDownloads.Select(async pair =>
            {
                var (categoryName, task) = pair;
                var results = await task;
                return (categoryName, results);
            }));

            var mirroredRepository = new IdeaPluginRepository();
            foreach (var (categoryName, downloadResults) in categoryResults)
            {
                var category = mirroredRepository.Categories.FirstOrDefault(c => c.Name == categoryName) ??
                               new IdeaPluginCategory(categoryName);
                if (!mirroredRepository.Categories.Contains(category))
                {
                    mirroredRepository.Categories.Add(category);
                }

                category.Plugins.AddRange(downloadResults.Where(r => r.IsSuccess).Select(r => r.Plugin));
            }

            var repoPath = Path.Combine(baseDirectory, "repository.xml");
            if (File.Exists(repoPath))
            {
                File.Delete(repoPath);
            }

            var serializer = new XmlSerializer(typeof(IdeaPluginRepository));
            using (var output = File.OpenWrite(repoPath))
            {
                serializer.Serialize(output, mirroredRepository);
            }
        }

        /// <summary>
        /// Mirrors the plugins compatible with the given product versions from the official repository.
        /// </summary>
        /// <param name="productVersions">The product versions.</param>
        /// <param name="ct">The cancellation token in use.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task MirrorRepositoriesAsync([NotNull] IEnumerable<string> productVersions, CancellationToken ct)
        {
            var repositories = new List<IdeaPluginRepository>();
            foreach (var productVersion in productVersions)
            {
                repositories.Add(await _api.ListPluginsAsync(productVersion, ct));
            }

            await MirrorRepositoriesAsync(repositories, ct);
        }

        /// <summary>
        /// Mirrors the given repositories, queueing their plugins up for download. This will merge the repositories
        /// together into a single filtered repository to save bandwidth.
        /// </summary>
        /// <param name="repositories">The repositories to mirror.</param>
        /// <param name="ct">The cancellation token in use.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task MirrorRepositoriesAsync([NotNull] IReadOnlyCollection<IdeaPluginRepository> repositories, CancellationToken ct)
        {
            await Console.Out.WriteLineAsync("Merging requested versioned repositories (this might take a while)...");

            var newCategories = new Dictionary<string, Dictionary<int, IdeaPlugin>>();

            var totalRepositories = repositories.Count;
            var mergedRepositories = 0;
            foreach (var repository in repositories)
            {
                foreach (var category in repository.Categories)
                {
                    if (!newCategories.TryGetValue(category.Name, out var newCategory))
                    {
                        newCategory = new Dictionary<int, IdeaPlugin>();
                        newCategories.Add(category.Name, newCategory);
                    }

                    foreach (var plugin in category.Plugins)
                    {
                        if (!newCategory.TryGetValue(plugin.GetIdentityHash(), out _))
                        {
                            newCategory.Add(plugin.GetIdentityHash(), plugin);
                        }
                    }
                }

                ++mergedRepositories;
                await Console.Out.WriteLineAsync($"Merged repository {mergedRepositories} out of {totalRepositories}.");
            }

            var mergedCategories = newCategories.Select
            (
                kvp =>
                    new IdeaPluginCategory
                    (
                        kvp.Key,
                        kvp.Value.Values.ToList()
                    )
            );

            var mergedRepository = new IdeaPluginRepository(mergedCategories.ToList());

            await MirrorRepositoryAsync(mergedRepository, ct);
        }

        /// <summary>
        /// Downloads the icon of the given plugin.
        /// </summary>
        /// <param name="targetDirectory">The directory to store the icon in.</param>
        /// <param name="plugin">The plugin to download the icon of.</param>
        /// <param name="theme">The theme variant to download.</param>
        /// <param name="ct">The cancellation token in use.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [ItemNotNull]
        private async Task<DownloadResult> DownloadIconAsync
        (
            string targetDirectory,
            [NotNull] IdeaPlugin plugin,
            [CanBeNull] string theme,
            CancellationToken ct
        )
        {
            // First, let's perform a quick existing file check against the values in the reported plugin
            var sluggedPluginName = plugin.Name.GenerateSlug();

            var saveDirectory = Path.Combine
            (
                targetDirectory,
                sluggedPluginName
            );

            if (!(theme is null))
            {
                saveDirectory = Path.Combine(saveDirectory, theme.GenerateSlug());
            }

            if (Directory.Exists(saveDirectory))
            {
                var existingFile = Directory.GetFiles(saveDirectory).FirstOrDefault();
                if (!(existingFile is null))
                {
                    if (new FileInfo(existingFile).Length != 0)
                    {
                        // Looks like we already have this one
                        return DownloadResult.FromSuccess(plugin, DownloadAction.Skipped);
                    }
                }
            }

            try
            {
                using (var data = await _api.DownloadIconAsync(plugin, theme, ct))
                {
                    if (!data.IsSuccessStatusCode)
                    {
                        return DownloadResult.FromError(plugin, DownloadError.Unknown, data.ReasonPhrase);
                    }

                    string filename = null;
                    if (data.Content.Headers?.ContentDisposition?.FileName is null)
                    {
                        // Try an alternate way
                        var alternatePath = data.RequestMessage?.RequestUri?.AbsolutePath;
                        if (!(alternatePath is null))
                        {
                            if (Path.HasExtension(alternatePath))
                            {
                                filename = Path.GetFileName(alternatePath);
                            }
                        }
                    }
                    else
                    {
                        filename = data.Content.Headers.ContentDisposition.FileName;
                    }

                    if (filename is null)
                    {
                        return DownloadResult.FromError
                        (
                            plugin,
                            DownloadError.Unknown,
                            "Failed to retrieve file information from the download headers."
                        );
                    }

                    Directory.CreateDirectory(saveDirectory);

                    var savePath = Path.Combine(saveDirectory, filename.Replace("\"", string.Empty));

                    if (File.Exists(savePath))
                    {
                        var expectedSize = data.Content.Headers?.ContentLength;
                        if (new FileInfo(savePath).Length == expectedSize)
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
            catch (TimeoutException tex)
            {
                return DownloadResult.FromError(plugin, DownloadError.Timeout, tex.Message);
            }
            catch (Exception ex)
            {
                return DownloadResult.FromError(plugin, DownloadError.Exception, ex.Message, ex);
            }
        }

        /// <summary>
        /// Downloads the given plugin.
        /// </summary>
        /// <param name="targetDirectory">The directory in which to save the plugin.</param>
        /// <param name="plugin">The plugin to download.</param>
        /// <param name="ct">The cancellation token in use.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [ItemNotNull]
        private async Task<DownloadResult> DownloadPluginAsync
        (
            string targetDirectory,
            [NotNull] IdeaPlugin plugin,
            CancellationToken ct
        )
        {
            // Try downloading the icon variants
            var iconTargetDirectory = Path.Combine(Program.Options.OutputFolder, "icons");
            await DownloadIconAsync(iconTargetDirectory, plugin, null, ct);
            await DownloadIconAsync(iconTargetDirectory, plugin, "DARCULA", ct);

            // First, let's perform a quick existing file check against the values in the reported plugin
            var sluggedPluginName = plugin.Name.GenerateSlug();
            var version = plugin.Version;

            var saveDirectory = Path.Combine
            (
                targetDirectory,
                sluggedPluginName,
                version
            );

            if (Directory.Exists(saveDirectory))
            {
                var existingFile = Directory.GetFiles(saveDirectory).FirstOrDefault();
                if (!(existingFile is null))
                {
                    if (new FileInfo(existingFile).Length == plugin.Size)
                    {
                        // Looks like we already have this one
                        return DownloadResult.FromSuccess(plugin, DownloadAction.Skipped);
                    }
                }
            }

            try
            {
                using (var data = await _api.DownloadAsync(plugin, ct))
                {
                    if (!data.IsSuccessStatusCode)
                    {
                        return DownloadResult.FromError(plugin, DownloadError.Unknown, data.ReasonPhrase);
                    }

                    string filename = null;
                    if (data.Content.Headers?.ContentDisposition?.FileName is null)
                    {
                        // Try an alternate way
                        var alternatePath = data.RequestMessage?.RequestUri?.AbsolutePath;
                        if (!(alternatePath is null))
                        {
                            if (Path.HasExtension(alternatePath))
                            {
                                filename = Path.GetFileName(alternatePath);
                            }
                        }
                    }
                    else
                    {
                        filename = data.Content.Headers.ContentDisposition.FileName;
                    }

                    if (filename is null)
                    {
                        return DownloadResult.FromError
                        (
                            plugin,
                            DownloadError.Unknown,
                            "Failed to retrieve file information from the download headers."
                        );
                    }

                    Directory.CreateDirectory(saveDirectory);

                    var savePath = Path.Combine(saveDirectory, filename.Replace("\"", string.Empty));

                    if (File.Exists(savePath))
                    {
                        var expectedSize = data.Content.Headers?.ContentLength ?? plugin.Size;
                        if (new FileInfo(savePath).Length == expectedSize)
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
            catch (TimeoutException tex)
            {
                return DownloadResult.FromError(plugin, DownloadError.Timeout, tex.Message);
            }
            catch (Exception ex)
            {
                return DownloadResult.FromError(plugin, DownloadError.Exception, ex.Message, ex);
            }
        }

        [ItemNotNull]
        private static async Task<DownloadResult> FinalizeDownload([NotNull] Task<DownloadResult> downloadTask)
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
                Console.ForegroundColor = ConsoleColor.Red;
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

                Console.ResetColor();

                return result;
            }

            switch (result.Action)
            {
                case DownloadAction.Downloaded:
                {
                    var printableVersion = "0.0.0";
                    var printableSize = string.Empty;
                    if (!(plugin is null))
                    {
                        var size = new ByteSize(plugin.Size);
                        printableSize = $"{size.LargestWholeNumberValue:F1} {size.LargestWholeNumberSymbol}";
                        printableVersion = plugin.Version;
                    }

                    Console.ForegroundColor = ConsoleColor.Green;
                    await Console.Out.WriteLineAsync
                    (
                        $"[{nameof(RepositoryMirrorer)}]: Downloaded {pluginName} v{printableVersion} ({printableSize})"
                    );

                    break;
                }

                case DownloadAction.Skipped:
                {
                    if (Program.Options.VerboseOutput)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
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

            Console.ResetColor();

            return result;
        }
    }
}
