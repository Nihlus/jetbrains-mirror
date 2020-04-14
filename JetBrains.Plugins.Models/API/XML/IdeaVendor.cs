//
//  IdeaVendor.cs
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

using System.Xml.Serialization;
using JetBrains.Annotations;

namespace JetBrains.Plugins.Models.API.XML
{
    /// <summary>
    /// Represents a plugin vendor.
    /// </summary>
    [PublicAPI]
    [XmlRoot(ElementName = "vendor")]
    public class IdeaVendor
    {
        /// <summary>
        /// Gets or sets the vendor's URL.
        /// </summary>
        [XmlAttribute(AttributeName = "url")]
        public string? URL { get; set; }

        /// <summary>
        /// Gets or sets the vendor's main contact email.
        /// </summary>
        [XmlAttribute(AttributeName = "email")]
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the vendor's name.
        /// </summary>
        [XmlText]
        public string? Name { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{this.URL} <{this.Email}>";
        }
    }
}
