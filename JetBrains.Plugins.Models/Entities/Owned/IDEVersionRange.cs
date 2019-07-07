//
//  IDEVersionRange.cs
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
        [CanBeNull]
        public virtual IDEVersion SinceBuild { get; set; }

        /// <summary>
        /// Gets or sets the upper exclusive bound of the compatibility range.
        /// </summary>
        [CanBeNull]
        public virtual IDEVersion UntilBuild { get; set; }

        /// <summary>
        /// Determines whether the given version is within with the range.
        /// </summary>
        /// <param name="version">The version to test.</param>
        /// <returns>true if the version falls inside of the range; otherwise, false.</returns>
        public bool IsInRange([NotNull] IDEVersion version)
        {
            if (this.SinceBuild is null && this.UntilBuild is null)
            {
                return false;
            }

            if (this.UntilBuild is null)
            {
                return version >= this.SinceBuild;
            }

            if (this.SinceBuild is null)
            {
                return version < this.UntilBuild;
            }

            return version >= this.SinceBuild && version < this.UntilBuild;
        }
    }
}
