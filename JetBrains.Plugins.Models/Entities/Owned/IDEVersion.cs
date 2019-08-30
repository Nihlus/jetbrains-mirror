//
//  IDEVersion.cs
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
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace JetBrains.Plugins.Models
{
    /// <summary>
    /// References a specific JetBrains IDE version (or implicit unbounded range).
    /// </summary>
    [PublicAPI, Owned]
    public class IDEVersion : IComparable<IDEVersion>, IComparable, IEquatable<IDEVersion>
    {
        /// <summary>
        /// Gets an instance that represents an invalid value.
        /// </summary>
        public static IDEVersion Invalid => new IDEVersion();

        /// <summary>
        /// Gets or sets the product ID of this version.
        /// </summary>
        [CanBeNull]
        public string ProductID { get; set; }

        /// <summary>
        /// Gets or sets the branch number.
        /// </summary>
        [Required]
        public int Branch { get; set; }

        /// <summary>
        /// Gets or sets the branch build number.
        /// </summary>
        [CanBeNull]
        public int? Build { get; set; }

        /// <summary>
        /// Gets or sets additional, more granular version numbers. These are appended in order after the main version
        /// structure, separated by dots.
        /// </summary>
        [NotNull]
        public virtual List<int> Extra { get; set; } = new List<int>();

        /// <summary>
        /// Gets a value indicating whether the version instance represents a valid value.
        /// </summary>
        [NotMapped]
        public bool IsValid => !(this.ProductID is null && this.Branch == 0);

        /// <summary>
        /// Initializes a new instance of the <see cref="IDEVersion"/> class.
        /// </summary>
        protected IDEVersion()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IDEVersion"/> class.
        /// </summary>
        /// <param name="branch">The branch number.</param>
        /// <param name="productID">The product ID.</param>
        /// <param name="build">The build number.</param>
        public IDEVersion(int branch, string productID = null, int? build = null)
        {
            this.Branch = branch;
            this.ProductID = productID;
            this.Build = build;
        }

        /// <summary>
        /// Parses an <see cref="IDEVersion"/> from the given string.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if the value was successfully parsed; otherwise, false.</returns>
        [Pure]
        public static bool TryParse(string value, out IDEVersion result)
        {
            int ParseBuildNumber(string buildComponent)
            {
                if (buildComponent.Length == 1 && buildComponent[0] == '*')
                {
                    return -1;
                }

                if (!int.TryParse(buildComponent, out var component))
                {
                    throw new InvalidDataException("Failed to parse a build number component.");
                }

                return component;
            }

            result = null;

            if (value == null)
            {
                return false;
            }

            if (value == "n/a")
            {
                result = Invalid;
                return true;
            }

            string productCode = null;
            var branch = 0;
            int? build = null;

            var productSeparator = value.IndexOf('-');
            if (productSeparator > 0)
            {
                productCode = value.Substring(0, productSeparator);
                value = value.Substring(productSeparator + 1);
            }

            var components = value.Split('.');

            var extra = new List<int>();
            for (var i = 0; i < components.Length; ++i)
            {
                switch (i)
                {
                    case 0:
                    {
                        branch = ParseBuildNumber(components[i]);
                        break;
                    }

                    case 1:
                    {
                        build = ParseBuildNumber(components[i]);
                        break;
                    }

                    default:
                    {
                        extra.Add(ParseBuildNumber(components[i]));
                        break;
                    }
                }
            }

            result = new IDEVersion
            {
                ProductID = productCode,
                Branch = branch,
                Build = build,
                Extra = extra
            };

            return true;
        }

        /// <inheritdoc/>
        public bool Equals([CanBeNull] IDEVersion other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(this.ProductID, other.ProductID) &&
                   this.Branch == other.Branch &&
                   this.Build == other.Build &&
                   this.Extra.SequenceEqual(other.Extra);
        }

        /// <inheritdoc/>
        public override bool Equals([CanBeNull] object obj)
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

            return Equals((IDEVersion)obj);
        }

        /// <inheritdoc/>
        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode", Justification = "Intentional")]
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.ProductID != null ? this.ProductID.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ this.Branch;
                hashCode = (hashCode * 397) ^ this.Build.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Extra.Aggregate((i, j) => (i * 397) ^ j.GetHashCode());
                return hashCode;
            }
        }

        /// <inheritdoc />
        public int CompareTo(IDEVersion other)
        {
            if (ReferenceEquals(this, other))
            {
                return 0;
            }

            if (ReferenceEquals(null, other))
            {
                return 1;
            }

            var branchComparison = this.Branch.CompareTo(other.Branch);
            if (branchComparison != 0)
            {
                return branchComparison;
            }

            var buildComparison = Nullable.Compare(this.Build, other.Build);
            if (buildComparison != 0)
            {
                return buildComparison;
            }

            foreach (var (i, j) in other.Extra.Zip(this.Extra, (i, j) => (i, j)))
            {
                var extraComparison = i.CompareTo(j);
                if (extraComparison != 0)
                {
                    return extraComparison;
                }
            }

            if (other.Extra.Count > this.Extra.Count)
            {
                return -1;
            }

            if (other.Extra.Count < this.Extra.Count)
            {
                return 1;
            }

            return 0;
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

            return obj is IDEVersion other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(IDEVersion)}");
        }

        /// <summary>
        /// Determines whether the given <see cref="IDEVersion"/> is logically lesser than the other IDEVersion.
        /// </summary>
        /// <param name="left">The first version.</param>
        /// <param name="right">The second version.</param>
        /// <returns>True if the left operand is logically lesser than the right operand; otherwise, false.</returns>
        public static bool operator <(IDEVersion left, IDEVersion right)
        {
            return left.CompareTo(right) < 0;
        }

        /// <summary>
        /// Determines whether the given <see cref="IDEVersion"/> is logically greater than the other IDEVersion.
        /// </summary>
        /// <param name="left">The first version.</param>
        /// <param name="right">The second version.</param>
        /// <returns>True if the left operand is logically greater than the right operand; otherwise, false.</returns>
        public static bool operator >(IDEVersion left, IDEVersion right)
        {
            return left.CompareTo(right) > 0;
        }

        /// <summary>
        /// Determines whether the given <see cref="IDEVersion"/> is logically lesser than or equal to the other
        /// IDEVersion.
        /// </summary>
        /// <param name="left">The first version.</param>
        /// <param name="right">The second version.</param>
        /// <returns>
        /// True if the left operand is logically lesser than or equal to the right operand; otherwise, false.
        /// </returns>
        public static bool operator <=(IDEVersion left, IDEVersion right)
        {
            return left.CompareTo(right) <= 0;
        }

        /// <summary>
        /// Determines whether the given <see cref="IDEVersion"/> is logically greater than or equal to the other
        /// IDEVersion.
        /// </summary>
        /// <param name="left">The first version.</param>
        /// <param name="right">The second version.</param>
        /// <returns>
        /// True if the left operand is logically greater than or equal to the right operand; otherwise, false.
        /// </returns>
        public static bool operator >=(IDEVersion left, IDEVersion right)
        {
            return left.CompareTo(right) >= 0;
        }

        /// <summary>
        /// Determines whether or not the given <see cref="IDEVersion"/> is logically equivalent to the other given
        /// IDEVersion.
        /// </summary>
        /// <param name="left">The first version.</param>
        /// <param name="right">The second version.</param>
        /// <returns>true if the operands are logically equivalent; otherwise, false.</returns>
        public static bool operator ==([CanBeNull] IDEVersion left, [CanBeNull] IDEVersion right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Determines whether or not the given <see cref="IDEVersion"/> is not logically equivalent to the other
        /// given IDEVersion.
        /// </summary>
        /// <param name="left">The first version.</param>
        /// <param name="right">The second version.</param>
        /// <returns>true if the operands are not logically equivalent; otherwise, false.</returns>
        public static bool operator !=([CanBeNull] IDEVersion left, [CanBeNull] IDEVersion right)
        {
            return !Equals(left, right);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            string ToVersionComponent(int value)
            {
                if (value < 0)
                {
                    return "*";
                }

                return value.ToString();
            }

            var sb = new StringBuilder();

            if (!(this.ProductID is null))
            {
                sb.Append(this.ProductID);
                sb.Append('-');
            }

            sb.Append(ToVersionComponent(this.Branch));
            if (sb[sb.Length - 1] == '*')
            {
                return sb.ToString();
            }

            if (!(this.Build is null))
            {
                sb.Append('.');
                sb.Append(ToVersionComponent(this.Build.Value));
                if (sb[sb.Length - 1] == '*')
                {
                    return sb.ToString();
                }
            }

            foreach (var extra in this.Extra)
            {
                sb.Append('.');
                sb.Append(ToVersionComponent(extra));

                if (sb[sb.Length - 1] == '*')
                {
                    return sb.ToString();
                }
            }

            return sb.ToString();
        }
    }
}
