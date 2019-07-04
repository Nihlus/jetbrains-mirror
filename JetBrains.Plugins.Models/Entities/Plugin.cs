//
//  Plugin.cs
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
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace JetBrains.Plugins.Models
{
    /// <summary>
    /// Represents a plugin in the database.
    /// </summary>
    public class Plugin : EFEntity
    {
        /// <summary>
        /// Gets or sets the name of the plugin.
        /// </summary>
        [Required, NotNull]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the category the plugin belongs to.
        /// </summary>
        [Required, NotNull]
        public PluginCategory Category { get; set; }

        /// <summary>
        /// Gets or sets the unique plugin identifier.
        /// </summary>
        [Required, NotNull]
        public string PluginID { get; set; }

        /// <summary>
        /// Gets or sets the description of the plugin.
        /// </summary>
        [Required, NotNull]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the plugin vendor.
        /// </summary>
        [Required, NotNull]
        public Vendor Vendor { get; set; }

        /// <summary>
        /// Gets or sets the list of released versions of this plugin.
        /// </summary>
        [Required, NotNull]
        public IList<PluginRelease> Releases { get; set; }

        /// <summary>
        /// Gets or sets the tags applied to the plugin.
        /// </summary>
        [Required, NotNull]
        public IList<string> Tags { get; set; }

        /// <summary>
        /// Gets or sets the community rating of the plugin.
        /// </summary>
        [Required]
        public double Rating { get; set; }

        /// <summary>
        /// Gets or sets the project URL of the plugin.
        /// </summary>
        [Required, NotNull]
        public string ProjectURL { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Plugin"/> class.
        /// </summary>
        /// <param name="name">The name of the plugin.</param>
        /// <param name="category">The category that the plugin belongs to.</param>
        /// <param name="pluginID">The unique ID of the plugin.</param>
        /// <param name="description">The description of the plugin.</param>
        /// <param name="vendor">The plugin vendor.</param>
        /// <param name="releases">The released versions of the plugin.</param>
        /// <param name="tags">The tags applied to the plugin.</param>
        /// <param name="rating">The rating of the plugin.</param>
        /// <param name="projectURL">The plugin's project URL.</param>
        public Plugin
        (
            [NotNull] string name,
            [NotNull] PluginCategory category,
            [NotNull] string pluginID,
            [NotNull] string description,
            [NotNull] Vendor vendor,
            [CanBeNull] IList<PluginRelease> releases = null,
            [CanBeNull] IList<string> tags = null,
            double rating = default,
            [CanBeNull] string projectURL = null
        )
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.Category = category ?? throw new ArgumentNullException(nameof(category));
            this.PluginID = pluginID ?? throw new ArgumentNullException(nameof(pluginID));
            this.Description = description ?? throw new ArgumentNullException(nameof(description));
            this.Vendor = vendor ?? throw new ArgumentNullException(nameof(vendor));
            this.Releases = releases ?? new List<PluginRelease>();
            this.Tags = tags ?? new List<string>();
            this.Rating = rating;
            this.ProjectURL = projectURL ?? string.Empty;
        }
    }
}
