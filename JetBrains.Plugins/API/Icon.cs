//
//  Icon.cs
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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Jetbrains.Plugins.Helpers;
using JetBrains.Plugins.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JetBrains.Plugins.API
{
    /// <summary>
    /// API endpoint for retrieving plugin icons.
    /// </summary>
    [Produces("application/octet-stream")]
    [Route("api/icon")]
    [ApiController]
    public class Icon : ControllerBase
    {
        [ProvidesContext]
        private PluginsDatabaseContext _database;

        /// <summary>
        /// Initializes a new instance of the <see cref="Icon"/> class.
        /// </summary>
        /// <param name="database">The database.</param>
        public Icon(PluginsDatabaseContext database)
        {
            _database = database;
        }

        /// <summary>
        /// Serves GET requests to this controller.
        /// </summary>
        /// <param name="pluginId">The ID of the plugin.</param>
        /// <param name="theme">The theme variant to request..</param>
        /// <returns>The plugins compatible with the given IDE version.</returns>
        public async Task<ActionResult<IEnumerable<string>>> Get(string pluginId, string? theme)
        {
            var plugin = await _database.Plugins.FirstOrDefaultAsync(p => p.PluginID == pluginId);

            if (plugin is null)
            {
                return NoContent();
            }

            const string testPath = "/run/media/jarl/seagate-expansion/repositories/jetbrains/icons";
            var iconDataPath = Path.Combine
            (
                testPath,
                plugin.Name.GenerateSlug()
            );

            if (!(theme is null))
            {
                iconDataPath = Path.Combine(iconDataPath, theme.GenerateSlug());
            }

            if (!Directory.Exists(iconDataPath))
            {
                return NoContent();
            }

            var dataFilePath = Directory.EnumerateFiles(iconDataPath).FirstOrDefault();
            if (dataFilePath is null)
            {
                return NoContent();
            }

            if (!System.IO.File.Exists(dataFilePath))
            {
                return NoContent();
            }

            var stream = System.IO.File.OpenRead(dataFilePath);
            return File(stream, "application/octet-stream", Path.GetFileName(dataFilePath));
        }
    }
}
