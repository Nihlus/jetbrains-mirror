//
//  IdeaPlugin.cs
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
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace JetBrains.Plugins.Models.API.XML
{
    /// <summary>
    /// Represents information about a single plugin.
    /// </summary>
    public class IdeaPlugin
    {
        /// <summary>
        /// Gets or sets the name of the plugin.
        /// </summary>
        [Required, NotNull]
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the ID of the plugin.
        /// </summary>
        [Required, NotNull]
        [XmlElement(ElementName = "id")]
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the description of the plugin.
        /// </summary>
        [Required, NotNull]
        [XmlElement(ElementName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the latest version of the plugin.
        /// </summary>
        [Required, NotNull]
        [XmlElement(ElementName = "version")]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the vendor that develops the plugin.
        /// </summary>
        [Required, NotNull]
        [XmlElement(ElementName = "vendor")]
        public Vendor Vendor { get; set; }

        /// <summary>
        /// Gets or sets the IDEA versions that the plugin supports.
        /// </summary>
        [Required, NotNull]
        [XmlElement(ElementName = "idea-version")]
        public IdeaVersion IdeaVersion { get; set; }

        /// <summary>
        /// Gets or sets the latest change notes.
        /// </summary>
        [Required, NotNull]
        [XmlElement(ElementName = "change-notes")]
        public string ChangeNotes { get; set; }

        /// <summary>
        /// Gets or sets the list of plugins this plugin depends on.
        /// </summary>
        [CanBeNull]
        [XmlElement(ElementName = "depends")]
        public List<string> Depends { get; set; }

        /// <summary>
        /// Gets or sets the tags applied to the plugin.
        /// </summary>
        [CanBeNull]
        [XmlElement(ElementName = "tags")]
        public string Tags { get; set; }

        /// <summary>
        /// Gets or sets the rating of the plugin.
        /// <remarks>This value ranges between 0 and 5.</remarks>
        /// </summary>
        [XmlElement(ElementName = "rating")]
        public double Rating { get; set; }

        /// <summary>
        /// Gets or sets the number of downloads that this plugin has had.
        /// </summary>
        [XmlAttribute(AttributeName = "downloads")]
        public ulong Downloads { get; set; }

        /// <summary>
        /// Gets or sets the size in bytes of the plugin.
        /// </summary>
        [XmlAttribute(AttributeName = "size")]
        public ulong Size { get; set; }

        /// <summary>
        /// Gets or sets the project URL of the plugin.
        /// </summary>
        [CanBeNull]
        [XmlAttribute(AttributeName = "url")]
        public string ProjectURL { get; set; }

        /// <summary>
        /// Gets or sets the initial upload date of the plugin.
        /// </summary>
        [CanBeNull]
        [XmlAttribute(AttributeName = "date")]
        public string UploadDate { get; set; }

        /// <summary>
        /// Gets or sets the latest update date of the plugin.
        /// </summary>
        [CanBeNull]
        [XmlAttribute(AttributeName = "updatedDate")]
        public string UpdateDate { get; set; }

        /// <summary>
        /// Gets a hash code that is unique to the ID-version combination of this plugin.
        /// </summary>
        /// <returns>The identity hash.</returns>
        public int GetIdentityHash()
        {
            unchecked
            {
                return (this.ID.GetHashCode() * 397) ^ this.Version.GetHashCode();
            }
        }

        /// <summary>
        /// Determines whether a given plugin represents the same logical plugin as this one.
        /// </summary>
        /// <param name="other">The other plugin.</param>
        /// <returns>true if the plugins are logically equivalent; otherwise, false.</returns>
        public bool IsSameAs([NotNull] IdeaPlugin other)
        {
            return GetIdentityHash() == other.GetIdentityHash();
        }

        /// <inheritdoc />
        [NotNull]
        public override string ToString()
        {
            return this.Name;
        }
    }
}
