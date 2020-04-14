//
//  Search.cs
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
using System.Linq.Expressions;
using JetBrains.Annotations;
using JetBrains.Plugins.Models;
using Microsoft.AspNetCore.Mvc;

namespace JetBrains.Plugins.API
{
    /// <summary>
    /// API endpoint for listing plugins compatible with a particular IDE version.
    /// </summary>
    [Produces("application/json")]
    [Route("api/search")]
    [ApiController]
    public class Search : ControllerBase
    {
        [ProvidesContext]
        private readonly PluginsDatabaseContext _database;

        /// <summary>
        /// Initializes a new instance of the <see cref="Search"/> class.
        /// </summary>
        /// <param name="database">The database context.</param>
        public Search(PluginsDatabaseContext database)
        {
            _database = database;
        }

        /// <summary>
        /// Serves GET requests to this controller.
        /// </summary>
        /// <param name="orderBy">What to order the search by.</param>
        /// <param name="build">The build to list plugins for.</param>
        /// <param name="max">The maximum number of results to return.</param>
        /// <param name="search">The search string.</param>
        /// <returns>The plugins compatible with the given IDE version.</returns>
        [NotNull]
        public ActionResult<IEnumerable<string>> Get(string orderBy, string build, int max, string search)
        {
            if (!IDEVersion.TryParse(build, out var ideVersion))
            {
                return BadRequest();
            }

            return new ActionResult<IEnumerable<string>>(GetResults(ideVersion, max, orderBy, search));
        }

        [NotNull]
        private IEnumerable<string> GetResults(IDEVersion version, int max, string orderBy, [CanBeNull] string search)
        {
            var compatibleIDs = new List<string>();

            IQueryable<Plugin> plugins = _database.Plugins;

            switch (orderBy)
            {
                case "update date":
                {
                    plugins = plugins.OrderByDescending(p => p.UpdatedAt);

                    break;
                }

                case "downloads":
                {
                    plugins = plugins
                        .OrderByDescending(p => p.Releases.Sum(r => r.Downloads));

                    break;
                }

                case "rating":
                {
                    plugins = plugins
                        .OrderByDescending(p => p.Rating);

                    break;
                }
            }

            foreach (var plugin in plugins)
            {
                if (compatibleIDs.Count >= max)
                {
                    break;
                }

                if (!plugin.Releases.Any(r => r.CompatibleWith.IsInRange(version)))
                {
                    continue;
                }

                if (search is null)
                {
                    compatibleIDs.Add(plugin.PluginID);
                    continue;
                }

                if (plugin.Name.Contains(search, StringComparison.OrdinalIgnoreCase))
                {
                    compatibleIDs.Add(plugin.PluginID);
                    continue;
                }

                if (plugin.PluginID.Contains(search, StringComparison.OrdinalIgnoreCase))
                {
                    compatibleIDs.Add(plugin.PluginID);
                }
            }

            return compatibleIDs;
        }
    }
}
