//
//  PluginVersion.cs
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
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace JetBrains.Plugins.Models
{
    /// <summary>
    /// Represents a specific plugin version. It's assumed to follow extended semver. Metadata is compared on an ordinal
    /// string sorting basis.
    ///
    /// This is an owned entity component, and may not appear as an independent entity.
    /// </summary>
    [Owned]
    public class PluginVersion : IComparable<PluginVersion>, IComparable, IEquatable<PluginVersion>
    {
        /// <summary>
        /// Gets or sets the major version of the plugin.
        /// </summary>
        public int Major { get; set; }

        /// <summary>
        /// Gets or sets the minor version of the plugin.
        /// </summary>
        public int Minor { get; set; }

        /// <summary>
        /// Gets or sets the patch version of the plugin.
        /// </summary>
        public int Patch { get; set; }

        /// <summary>
        /// Gets or sets any extra version data, such as prerelease or metadata information. This is directly appended
        /// to the version string.
        /// </summary>
        public string Extra { get; set; }

        /// <inheritdoc />
        public bool Equals(PluginVersion other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.Major == other.Major && this.Minor == other.Minor && this.Patch == other.Patch && string.Equals(this.Extra, other.Extra);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((PluginVersion)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.Major;
                hashCode = (hashCode * 397) ^ this.Minor;
                hashCode = (hashCode * 397) ^ this.Patch;
                hashCode = (hashCode * 397) ^ (this.Extra != null ? this.Extra.GetHashCode() : 0);
                return hashCode;
            }
        }

        /// <inheritdoc/>
        public int CompareTo(PluginVersion other)
        {
            if (ReferenceEquals(this, other))
            {
                return 0;
            }

            if (ReferenceEquals(null, other))
            {
                return 1;
            }

            var majorComparison = this.Major.CompareTo(other.Major);
            if (majorComparison != 0)
            {
                return majorComparison;
            }

            var minorComparison = this.Minor.CompareTo(other.Minor);
            if (minorComparison != 0)
            {
                return minorComparison;
            }

            var patchComparison = this.Patch.CompareTo(other.Patch);
            if (patchComparison != 0)
            {
                return patchComparison;
            }

            return string.Compare(this.Extra, other.Extra, StringComparison.Ordinal);
        }

        /// <inheritdoc/>
        public int CompareTo([CanBeNull] object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return 1;
            }

            if (ReferenceEquals(this, obj))
            {
                return 0;
            }

            return obj is PluginVersion other ?
                CompareTo(other) :
                throw new ArgumentException($"Object must be of type {nameof(PluginVersion)}");
        }

        /// <summary>
        /// Determines whether the given <see cref="PluginVersion"/> is logically lesser than the other PluginVersion.
        /// </summary>
        /// <param name="left">The first version.</param>
        /// <param name="right">The second version.</param>
        /// <returns>True if the left operand is logically lesser than the right operand; otherwise, false.</returns>
        public static bool operator <(PluginVersion left, PluginVersion right)
        {
            return Comparer<PluginVersion>.Default.Compare(left, right) < 0;
        }

        /// <summary>
        /// Determines whether the given <see cref="PluginVersion"/> is logically greater than the other PluginVersion.
        /// </summary>
        /// <param name="left">The first version.</param>
        /// <param name="right">The second version.</param>
        /// <returns>True if the left operand is logically greater than the right operand; otherwise, false.</returns>
        public static bool operator >(PluginVersion left, PluginVersion right)
        {
            return Comparer<PluginVersion>.Default.Compare(left, right) > 0;
        }

        /// <summary>
        /// Determines whether the given <see cref="PluginVersion"/> is logically lesser than or equal to the other
        /// PluginVersion.
        /// </summary>
        /// <param name="left">The first version.</param>
        /// <param name="right">The second version.</param>
        /// <returns>
        /// True if the left operand is logically lesser than or equal to the right operand; otherwise, false.
        /// </returns>
        public static bool operator <=(PluginVersion left, PluginVersion right)
        {
            return Comparer<PluginVersion>.Default.Compare(left, right) <= 0;
        }

        /// <summary>
        /// Determines whether the given <see cref="PluginVersion"/> is logically greater than or equal to the other
        /// PluginVersion.
        /// </summary>
        /// <param name="left">The first version.</param>
        /// <param name="right">The second version.</param>
        /// <returns>
        /// True if the left operand is logically greater than or equal to the right operand; otherwise, false.
        /// </returns>
        public static bool operator >=(PluginVersion left, PluginVersion right)
        {
            return Comparer<PluginVersion>.Default.Compare(left, right) >= 0;
        }

        /// <summary>
        /// Determines whether or not the given <see cref="PluginVersion"/> is logically equivalent to the other given
        /// PluginVersion.
        /// </summary>
        /// <param name="left">The first version.</param>
        /// <param name="right">The second version.</param>
        /// <returns>true if the operands are logically equivalent; otherwise, false.</returns>
        public static bool operator ==([CanBeNull] PluginVersion left, [CanBeNull] PluginVersion right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Determines whether or not the given <see cref="PluginVersion"/> is not logically equivalent to the other
        /// given PluginVersion.
        /// </summary>
        /// <param name="left">The first version.</param>
        /// <param name="right">The second version.</param>
        /// <returns>true if the operands are not logically equivalent; otherwise, false.</returns>
        public static bool operator !=([CanBeNull] PluginVersion left, [CanBeNull] PluginVersion right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Implicitly converts an instance of a <see cref="PluginVersion"/> class to a <see cref="Version"/> object.
        /// </summary>
        /// <param name="this">The plugin version to convert.</param>
        /// <returns>The converted instance.</returns>
        [NotNull]
        public static implicit operator Version([NotNull] PluginVersion @this)
        {
            return new Version(@this.Major, @this.Minor, @this.Patch);
        }
    }
}
