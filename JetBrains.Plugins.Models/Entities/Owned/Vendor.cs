//
//  Vendor.cs
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

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace JetBrains.Plugins.Models
{
    /// <summary>
    /// Represents a plugin vendor.
    /// </summary>
    [PublicAPI, Owned]
    public class Vendor
    {
        /// <summary>
        /// Gets or sets the name of the vendor.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the primary URL of the vendor.
        /// </summary>
        public string? URL { get; set; }

        /// <summary>
        /// Gets or sets the primary contact email of the vendor.
        /// </summary>
        public string? Email { get; set; }
    }
}
