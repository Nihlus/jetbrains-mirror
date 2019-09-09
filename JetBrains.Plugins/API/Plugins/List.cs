//
//  List.cs
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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using JetBrains.Annotations;
using JetBrains.Plugins.Models;
using JetBrains.Plugins.Models.API.XML;
using Microsoft.AspNetCore.Mvc;

namespace JetBrains.Plugins.API.Plugins
{
    /// <summary>
    /// API endpoint for listing plugins compatible with a particular IDE version.
    /// </summary>
    [Produces("application/xml")]
    [Route("plugins/list")]
    [ApiController]
    public class List : ControllerBase
    {
        [ProvidesContext]
        private readonly PluginsDatabaseContext _database;

        /// <summary>
        /// Initializes a new instance of the <see cref="List"/> class.
        /// </summary>
        /// <param name="database">The database context.</param>
        public List(PluginsDatabaseContext database)
        {
            _database = database;
        }

        /// <summary>
        /// Serves GET requests to this controller.
        /// </summary>
        /// <param name="build">The build to list plugins for.</param>
        /// <returns>The plugins compatible with the given IDE version.</returns>
        [NotNull]
        public ActionResult<IdeaPluginRepository> Get(string build)
        {
            if (!IDEVersion.TryParse(build, out var ideVersion))
            {
                return BadRequest();
            }

            var categories = _database.Categories.ToList();
            var ideaCategories = categories.Select(c => new IdeaPluginCategory(c.Name)).ToList();

            foreach (var category in _database.Categories)
            {
                var ideaCategory = ideaCategories.First(c => c.Name == category.Name);

                var plugins = category.Plugins.Select
                (
                    p =>
                    {
                        var compatibleRelease = p.Releases
                            .OrderBy(r => r.UploadedAt)
                            .FirstOrDefault(r => r.CompatibleWith.IsInRange(ideVersion));

                        if (compatibleRelease is null)
                        {
                            return null;
                        }

                        var ideaPlugin = new IdeaPlugin
                        (
                            p.Name,
                            p.PluginID,
                            p.Description,
                            compatibleRelease.Version,
                            new IdeaVendor
                            {
                                Email = p.Vendor.Email,
                                Name = p.Vendor.Name,
                                URL = p.Vendor.URL
                            },
                            new IdeaVersion
                            {
                                Min = "n/a",
                                Max = "n/a",
                                UntilBuild = compatibleRelease.CompatibleWith.UntilBuild.ToString(),
                                SinceBuild = compatibleRelease.CompatibleWith.SinceBuild.ToString()
                            },
                            compatibleRelease.ChangeNotes
                        )
                        {
                            Depends = compatibleRelease.Dependencies,
                            Downloads = compatibleRelease.Downloads,
                            Size = compatibleRelease.Size,
                            Rating = p.Rating,
                            UploadDate = compatibleRelease.UploadedAt.ToFileTimeUtc().ToString()
                        };

                        return ideaPlugin;
                    }
                )
                .Where(p => !(p is null));

                ideaCategory.Plugins = plugins.ToList();
            }

            var ideaRepository = new IdeaPluginRepository();
            ideaRepository.Categories = ideaCategories;

            return new ActionResult<IdeaPluginRepository>(ideaRepository);
        }
    }
}
