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

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace JetBrains.Plugins.Models
{
    /// <summary>
    /// Represents a plugin in the database.
    /// </summary>
    [PublicAPI]
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
        public List<PluginRelease> Releases { get; set; }

        /// <summary>
        /// Gets or sets the tags applied to the plugin.
        /// </summary>
        [Required, NotNull]
        public List<string> Tags { get; set; }

        /// <summary>
        /// Gets or sets the community rating of the plugin.
        /// </summary>
        [Required]
        public double Rating { get; set; }

        /// <summary>
        /// Gets or sets the project URL of the plugin.
        /// </summary>
        [Required, NotNull]
        public string ProjectURL { get; set; } = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="Plugin"/> class.
        /// </summary>
        [UsedImplicitly]
        [SuppressMessage("ReSharper", "NotNullMemberIsNotInitialized", Justification = "Initialized by EF Core.")]
        protected Plugin()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Plugin"/> class.
        /// </summary>
        /// <param name="name">The name of the plugin.</param>
        /// <param name="category">The category that the plugin is in.</param>
        /// <param name="pluginID">The unique ID of the plugin.</param>
        /// <param name="description">The plugin's description.</param>
        /// <param name="vendor">The plugin's vendor.</param>
        public Plugin
        (
            [NotNull] string name,
            [NotNull] PluginCategory category,
            [NotNull] string pluginID,
            [NotNull] string description,
            [NotNull] Vendor vendor
        )
        {
            this.Name = name;
            this.Category = category;
            this.PluginID = pluginID;
            this.Description = description;
            this.Vendor = vendor;

            this.Releases = new List<PluginRelease>();
            this.Tags = new List<string>();
            this.Rating = 0.0;
            this.ProjectURL = string.Empty;
        }
    }
}
