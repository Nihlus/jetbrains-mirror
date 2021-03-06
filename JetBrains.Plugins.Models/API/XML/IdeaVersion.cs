//
//  IdeaVersion.cs
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
    /// Represents the version of the JetBrains IDE the plugin is compatible with.
    /// </summary>
    [PublicAPI]
    [XmlRoot(ElementName = "idea-version")]
    public class IdeaVersion
    {
        /// <summary>
        /// Gets or sets the minimum version the plugin is compatible with.
        /// </summary>
        [XmlAttribute(AttributeName = "min")]
        public string? Min { get; set; }

        /// <summary>
        /// Gets or sets the maximum version the plugin is compatible with.
        /// </summary>
        [XmlAttribute(AttributeName = "max")]
        public string? Max { get; set; }

        /// <summary>
        /// Gets or sets the build the plugin is compatible from.
        /// </summary>
        [XmlAttribute(AttributeName = "since-build")]
        public string? SinceBuild { get; set; }

        /// <summary>
        /// Gets or sets the build the plugin stops being compatible with.
        /// </summary>
        [XmlAttribute(AttributeName = "until-build")]
        public string? UntilBuild { get; set; }
    }
}
