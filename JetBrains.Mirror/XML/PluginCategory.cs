//
//  PluginCategory.cs
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
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace JetBrains.Mirror.XML
{
    /// <summary>
    /// Represents a category of plugins.
    /// </summary>
    public class PluginCategory
    {
        /// <summary>
        /// Gets or sets the name of the category.
        /// </summary>
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the list of plugins in the category.
        /// </summary>
        [XmlElement(ElementName = "idea-plugin")]
        public List<IdeaPlugin> Plugins { get; set; } = new List<IdeaPlugin>();

        /// <inheritdoc />
        [NotNull]
        public override string ToString()
        {
            return $"{this.Name} ({this.Plugins.Count} plugins)";
        }
    }
}
