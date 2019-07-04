//
//  PluginRelease.cs
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
    /// Represents a single versioned release of a plugin.
    /// </summary>
    public class PluginRelease : EFEntity
    {
        /// <summary>
        /// Gets or sets the parent plugin.
        /// </summary>
        [Required, NotNull]
        public Plugin Plugin { get; set; }

        /// <summary>
        /// Gets or sets the change notes for this release.
        /// </summary>
        [Required, NotNull]
        public string ChangeNotes { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the number of times this release has been downloaded.
        /// </summary>
        [Required]
        public ulong Downloads { get; set; }

        /// <summary>
        /// Gets or sets the size (in bytes) of the plugin.
        /// </summary>
        [Required]
        public ulong Size { get; set; }

        /// <summary>
        /// Gets or sets the time at which this release was uploaded.
        /// </summary>
        [Required]
        public DateTime UploadedAt { get; set; }

        /// <summary>
        /// Gets or sets the MD5 hash of the file associated with this release.
        /// </summary>
        [Required, NotNull]
        public string Hash { get; set; }

        /// <summary>
        /// Gets or sets the version of this release.
        /// </summary>
        [Required, NotNull]
        public PluginVersion Version { get; set; }

        /// <summary>
        /// Gets or sets the range of IDE builds that this release is compatible with.
        /// </summary>
        [Required, NotNull]
        public IDEVersionRange CompatibleWith { get; set; }

        /// <summary>
        /// Gets or sets the list of plugins that this plugin depends on.
        /// </summary>
        [NotNull]
        public List<PluginDependency> Dependencies { get; set; } = new List<PluginDependency>();
    }
}
