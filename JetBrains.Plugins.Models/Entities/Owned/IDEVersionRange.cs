//
//  IDEVersionRange.cs
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

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

// ReSharper disable NotNullMemberIsNotInitialized - EF entities rely on data mapping.
namespace JetBrains.Plugins.Models
{
    /// <summary>
    /// Represents a range of supported IDE versions.
    /// </summary>
    [PublicAPI, Owned]
    public class IDEVersionRange
    {
        /// <summary>
        /// Gets or sets the lower inclusive bound of the compatibility range.
        /// </summary>
        [Required]
        public virtual IDEVersion SinceBuild { get; set; }

        /// <summary>
        /// Gets or sets the upper exclusive bound of the compatibility range.
        /// </summary>
        [Required]
        public virtual IDEVersion UntilBuild { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IDEVersionRange"/> class.
        /// </summary>
        /// <param name="sinceBuild">The initial build.</param>
        /// <param name="untilBuild">The final build.</param>
        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Intentional.")]
        public IDEVersionRange(IDEVersion sinceBuild, IDEVersion untilBuild)
        {
            this.SinceBuild = sinceBuild;
            this.UntilBuild = untilBuild;
        }

        /// <summary>
        /// Determines whether the given version is within with the range.
        /// </summary>
        /// <param name="version">The version to test.</param>
        /// <returns>true if the version falls inside of the range; otherwise, false.</returns>
        public bool IsInRange(IDEVersion version)
        {
            if (!this.SinceBuild.IsValid && !this.UntilBuild.IsValid)
            {
                return false;
            }

            if (!this.UntilBuild.IsValid)
            {
                return version >= this.SinceBuild;
            }

            if (!this.SinceBuild.IsValid)
            {
                return version < this.UntilBuild;
            }

            return version >= this.SinceBuild && version < this.UntilBuild;
        }
    }
}
