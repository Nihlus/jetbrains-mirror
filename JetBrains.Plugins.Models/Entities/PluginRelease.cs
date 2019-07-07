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
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace JetBrains.Plugins.Models
{
    /// <summary>
    /// Represents a single versioned release of a plugin.
    /// </summary>
    [PublicAPI]
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
        public string ChangeNotes { get; set; }

        /// <summary>
        /// Gets or sets the number of times this release has been downloaded.
        /// </summary>
        [Required]
        public long Downloads { get; set; }

        /// <summary>
        /// Gets or sets the size (in bytes) of the plugin.
        /// </summary>
        [Required]
        public long Size { get; set; }

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
        /// Gets or sets the version of this release. This field, while typically semver, can follow a variety of
        /// loosely defined formats.
        /// </summary>
        [Required, NotNull]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the range of IDE builds that this release is compatible with.
        /// </summary>
        [Required, NotNull]
        public IDEVersionRange CompatibleWith { get; set; }

        /// <summary>
        /// Gets or sets the list of plugin IDs that this plugin depends on.
        /// </summary>
        /// <remarks>This list can be empty.</remarks>
        [Required, NotNull]
        public List<string> Dependencies { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginRelease"/> class.
        /// </summary>
        [UsedImplicitly]
        [SuppressMessage("ReSharper", "NotNullMemberIsNotInitialized", Justification = "Initialized by EF Core.")]
        protected PluginRelease()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginRelease"/> class.
        /// </summary>
        /// <param name="plugin">The plugin that this release is of.</param>
        /// <param name="changeNotes">The changelog of this release.</param>
        /// <param name="size">The size (in bytes) of the release file.</param>
        /// <param name="uploadedAt">The time at which the release was uploaded.</param>
        /// <param name="hash">The MD5 hash of the release file.</param>
        /// <param name="version">The version of this release.</param>
        /// <param name="compatibleWith">The IDE versions this release is compatible with.</param>
        /// <param name="dependencies">The dependencies of this release.</param>
        public PluginRelease
        (
            [NotNull] Plugin plugin,
            [NotNull] string changeNotes,
            long size,
            DateTime uploadedAt,
            [NotNull] string hash,
            [NotNull] string version,
            [NotNull] IDEVersionRange compatibleWith,
            [CanBeNull] List<string> dependencies = null
        )
        {
            dependencies = dependencies ?? new List<string>();

            this.Plugin = plugin;
            this.ChangeNotes = changeNotes;
            this.Size = size;
            this.UploadedAt = uploadedAt;
            this.Hash = hash;
            this.Version = version;
            this.CompatibleWith = compatibleWith;

            this.Dependencies = dependencies;
        }
    }
}
