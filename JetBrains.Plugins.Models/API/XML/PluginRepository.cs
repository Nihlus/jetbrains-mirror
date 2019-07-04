//
//  PluginRepository.cs
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

namespace JetBrains.Plugins.Models.API.XML
{
    /// <summary>
    /// Represents an information listing about the official JetBrains plugin repository.
    /// </summary>
    [XmlRoot(ElementName = "plugin-repository")]
    public class PluginRepository
    {
        /// <summary>
        /// Gets or sets the categories in the repository.
        /// </summary>
        [XmlElement(ElementName = "category")]
        public List<PluginCategory> Categories { get; set; } = new List<PluginCategory>();
    }
}
