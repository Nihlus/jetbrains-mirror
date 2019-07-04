//
//  PluginCategory.cs
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
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace JetBrains.Plugins.Models
{
    /// <summary>
    /// Represents a category of plugins.
    /// </summary>
    public class PluginCategory : EFEntity
    {
        /// <summary>
        /// Gets or sets the name of the category.
        /// </summary>
        [Required, NotNull]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the list of plugins in this category.
        /// </summary>
        [Required, NotNull]
        public List<Plugin> Plugins { get; set; } = new List<Plugin>();
    }
}