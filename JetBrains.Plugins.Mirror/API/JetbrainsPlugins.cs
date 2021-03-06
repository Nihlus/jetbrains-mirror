//
//  JetbrainsPlugins.cs
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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;
using JetBrains.Annotations;
using JetBrains.Plugins.Models.API.XML;

namespace JetBrains.Plugins.Mirror.API
{
    /// <summary>
    /// Acts as an API handle to the official JetBrains plugin repository.
    /// </summary>
    public class JetbrainsPlugins
    {
        private readonly HttpClient _httpClient;
        private static readonly XmlSerializer RepositorySerializer;

        private readonly string _baseURL;

        static JetbrainsPlugins()
        {
            RepositorySerializer = new XmlSerializer(typeof(IdeaPluginRepository));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JetbrainsPlugins"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client to use.</param>
        /// <param name="baseURL">The base URL to connect to.</param>
        public JetbrainsPlugins(HttpClient httpClient, string baseURL = Endpoints.BaseURL)
        {
            _httpClient = httpClient;
            _baseURL = baseURL;
        }

        /// <summary>
        /// Lists the plugins available for the given product of the specified version.
        /// </summary>
        /// <param name="productBuild">The product to list the plugins for.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>The repository object representing the plugins available for the given product.</returns>
        public async Task<IdeaPluginRepository> ListPluginsAsync(string productBuild, CancellationToken ct)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query[Endpoints.PluginList.Parameters.Build] = productBuild;

            var uriBuilder = new UriBuilder("https", _baseURL)
            {
                Path = Endpoints.PluginList.BasePath,
                Query = query.ToString() ?? string.Empty
            };

            using var response = await _httpClient.GetAsync(uriBuilder.Uri, ct);
            await using var stream = await response.Content.ReadAsStreamAsync();

            return (IdeaPluginRepository)RepositorySerializer.Deserialize(stream);
        }

        /// <summary>
        /// Lists all versions available for the given plugin.
        /// </summary>
        /// <param name="pluginId">The ID of the plugin to list the versions for.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>A list of plugin versions.</returns>
        [ItemNotNull]
        public async Task<IReadOnlyList<IdeaPlugin>> ListVersionsAsync(string pluginId, CancellationToken ct)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query[Endpoints.PluginList.Parameters.PluginID] = pluginId;

            var uriBuilder = new UriBuilder("https", _baseURL)
            {
                Path = Endpoints.PluginList.BasePath,
                Query = query.ToString() ?? string.Empty
            };

            using var response = await _httpClient.GetAsync(uriBuilder.Uri, ct);
            await using var stream = await response.Content.ReadAsStreamAsync();
            var repository = (IdeaPluginRepository)RepositorySerializer.Deserialize(stream);

            return repository.Categories.SelectMany(c => c.Plugins).ToList();
        }

        /// <summary>
        /// Downloads the icon of the given plugin.
        /// </summary>
        /// <param name="plugin">The plugin to download the icon of.</param>
        /// <param name="theme">The theme variant to download.</param>
        /// <param name="ct">The cancellation token in use.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [ItemNotNull]
        public async Task<HttpResponseMessage> DownloadIconAsync
        (
            IdeaPlugin plugin,
            string? theme,
            CancellationToken ct
        )
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query[Endpoints.API.Icon.Parameters.PluginID] = plugin.ID;

            if (!(theme is null))
            {
                query[Endpoints.API.Icon.Parameters.Theme] = theme.ToUpperInvariant();
            }

            var uriBuilder = new UriBuilder("https", _baseURL)
            {
                Path = Endpoints.API.Icon.BasePath,
                Query = query.ToString() ?? string.Empty
            };

            // Resolve moved requests.
            var data = await _httpClient.GetAsync(uriBuilder.Uri, HttpCompletionOption.ResponseHeadersRead, ct);
            while (!data.IsSuccessStatusCode && (data.StatusCode == HttpStatusCode.MovedPermanently || data.StatusCode == HttpStatusCode.Found))
            {
                var newData = await _httpClient.GetAsync(data.Headers.Location, HttpCompletionOption.ResponseHeadersRead, ct);
                data.Dispose();
                data = newData;
            }

            return data;
        }

        /// <summary>
        /// Downloads the given plugin.
        /// </summary>
        /// <param name="plugin">The plugin.</param>
        /// <param name="ct">The cancellation token to use.</param>
        /// <returns>The response from the server.</returns>
        public Task<HttpResponseMessage> DownloadAsync(IdeaPlugin plugin, CancellationToken ct) =>
            DownloadSpecificAsync(plugin.ID, plugin.Version, ct);

        /// <summary>
        /// Downloads the given plugin.
        /// </summary>
        /// <param name="pluginId">The ID of the plugin.</param>
        /// <param name="productBuild">The product build to get the latest version for.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>The response from the server.</returns>
        [ItemNotNull]
        public async Task<HttpResponseMessage> DownloadLatestAsync(string pluginId, string productBuild, CancellationToken ct)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query[Endpoints.PluginManager.Parameters.Action] = "download";
            query[Endpoints.PluginManager.Parameters.ID] = pluginId;
            query[Endpoints.PluginManager.Parameters.Build] = productBuild;

            var uriBuilder = new UriBuilder("https", _baseURL)
            {
                Path = Endpoints.PluginManager.BasePath,
                Query = query.ToString() ?? string.Empty
            };

            // Resolve moved requests.
            var data = await _httpClient.GetAsync(uriBuilder.Uri, HttpCompletionOption.ResponseHeadersRead, ct);
            while (!data.IsSuccessStatusCode && data.StatusCode == HttpStatusCode.MovedPermanently)
            {
                var newData = await _httpClient.GetAsync(data.Headers.Location, HttpCompletionOption.ResponseHeadersRead, ct);

                data.Dispose();
                data = newData;
            }

            return data;
        }

        /// <summary>
        /// Downloads the given plugin.
        /// </summary>
        /// <param name="pluginId">The ID of the plugin.</param>
        /// <param name="version">The specific version to download.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>The response from the server.</returns>
        [ItemNotNull]
        public async Task<HttpResponseMessage> DownloadSpecificAsync(string pluginId, string version, CancellationToken ct)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query[Endpoints.PluginDownload.Parameters.PluginID] = pluginId;
            query[Endpoints.PluginDownload.Parameters.Version] = version;

            var uriBuilder = new UriBuilder("https", _baseURL)
            {
                Path = Endpoints.PluginDownload.BasePath,
                Query = query.ToString() ?? string.Empty
            };

            // Resolve moved requests.
            var data = await _httpClient.GetAsync(uriBuilder.Uri, HttpCompletionOption.ResponseHeadersRead, ct);
            while (!data.IsSuccessStatusCode && data.StatusCode == HttpStatusCode.MovedPermanently)
            {
                var newData = await _httpClient.GetAsync(data.Headers.Location, HttpCompletionOption.ResponseHeadersRead, ct);

                data.Dispose();
                data = newData;
            }

            return data;
        }
    }
}
