//
//  PluginManager.cs
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
using JetBrains.Annotations;
using Jetbrains.Plugins.Helpers;
using JetBrains.Plugins.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JetBrains.Plugins.API
{
    /// <summary>
    /// API endpoint for listing plugins compatible with a particular IDE version.
    /// </summary>
    [Produces("application/octet-stream")]
    [Route("pluginManager")]
    [ApiController]
    public class PluginManager : ControllerBase
    {
        [ProvidesContext]
        private readonly PluginsDatabaseContext _database;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginManager"/> class.
        /// </summary>
        /// <param name="database">The database context.</param>
        public PluginManager(PluginsDatabaseContext database)
        {
            _database = database;
        }

        /// <summary>
        /// Serves GET requests to this controller.
        /// </summary>
        /// <param name="id">The ID of the plugin.</param>
        /// <param name="build">The build to list plugins for.</param>
        /// <returns>The plugins compatible with the given IDE version.</returns>
        public async Task<ActionResult<IEnumerable<string>>> Get(string id, string build)
        {
            if (!IDEVersion.TryParse(build, out var ideVersion))
            {
                return BadRequest();
            }

            var plugin = await _database.Plugins
                    .FirstOrDefaultAsync(p => p.PluginID.Equals(id, StringComparison.OrdinalIgnoreCase));

            if (plugin is null)
            {
                return NotFound();
            }

            var compatibleRelease = plugin.Releases.FirstOrDefault(r => r.CompatibleWith.IsInRange(ideVersion));
            if (compatibleRelease is null)
            {
                return NotFound();
            }

            const string testPath = "/run/media/jarl/seagate-expansion/repositories/jetbrains/plugins";
            var pluginDataPath = Path.Combine
            (
                testPath,
                plugin.Category.Name.GenerateSlug(),
                plugin.Name.GenerateSlug(),
                compatibleRelease.Version
            );

            var dataFilePath = Directory.EnumerateFiles(pluginDataPath).FirstOrDefault();
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
