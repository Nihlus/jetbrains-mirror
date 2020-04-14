//
//  IdeaPluginCategory.cs
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

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace JetBrains.Plugins.Models.API.XML
{
    /// <summary>
    /// Represents a category of plugins.
    /// </summary>
    [PublicAPI]
    [XmlRoot(ElementName = "category")]
    public class IdeaPluginCategory
    {
        /// <summary>
        /// Gets or sets the name of the category.
        /// </summary>
        [Required]
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets the list of plugins in the category.
        /// </summary>
        [XmlElement(ElementName = "idea-plugin")]
        public List<IdeaPlugin> Plugins { get; set; } = null!;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdeaPluginCategory"/> class.
        /// </summary>
        [UsedImplicitly]
        [SuppressMessage("ReSharper", "NotNullMemberIsNotInitialized", Justification = "Initialized by XmlSerializer.")]
        protected IdeaPluginCategory()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdeaPluginCategory"/> class.
        /// </summary>
        /// <param name="name">The name of the category.</param>
        /// <param name="plugins">The plugins in the category.</param>
        public IdeaPluginCategory(string name, List<IdeaPlugin>? plugins = null)
        {
            plugins ??= new List<IdeaPlugin>();

            this.Name = name;
            this.Plugins = plugins;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{this.Name} ({this.Plugins.Count} plugins)";
        }
    }
}
