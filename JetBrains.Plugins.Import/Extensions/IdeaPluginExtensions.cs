//
//  IdeaPluginExtensions.cs
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
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Plugins.Models;
using JetBrains.Plugins.Models.API.XML;

namespace JetBrains.Plugins.Import.Extensions
{
    /// <summary>
    /// Extension methods for the <see cref="IdeaPlugin"/> class.
    /// </summary>
    public static class IdeaPluginExtensions
    {
        /// <summary>
        /// Maps the given <see cref="IdeaPlugin"/> to a <see cref="Plugin"/>.
        /// </summary>
        /// <param name="this">The IdeaPlugin.</param>
        /// <param name="dbCategory">The plugin category.</param>
        /// <returns>The mapped plugin.</returns>
        [NotNull]
        public static Plugin ToEntity
        (
            [NotNull] this IdeaPlugin @this,
            [NotNull] Models.PluginCategory dbCategory
        )
        {
            return new Plugin
            {
                Name = @this.Name,
                Category = dbCategory,
                Vendor = @this.Vendor.ToEntity(),
                PluginID = @this.ID,
                Description = @this.Description,
                Tags = @this.Tags?.Split(';').ToList() ?? new List<string>(),
                Rating = @this.Rating,
                ProjectURL = @this.ProjectURL ?? string.Empty,
                Releases = new List<PluginRelease>()
            };
        }
    }
}
