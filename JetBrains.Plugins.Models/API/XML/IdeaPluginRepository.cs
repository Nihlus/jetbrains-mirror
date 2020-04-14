//
//  IdeaPluginRepository.cs
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
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace JetBrains.Plugins.Models.API.XML
{
    /// <summary>
    /// Represents an information listing about the official JetBrains plugin repository.
    /// </summary>
    [PublicAPI]
    [XmlRoot(ElementName = "plugin-repository")]
    public class IdeaPluginRepository
    {
        /// <summary>
        /// Gets or sets the categories in the repository.
        /// </summary>
        [XmlElement(ElementName = "category")]
        public List<IdeaPluginCategory> Categories { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdeaPluginRepository"/> class.
        /// </summary>
        [UsedImplicitly]
        [SuppressMessage("ReSharper", "NotNullMemberIsNotInitialized", Justification = "Initialized by XmlSerializer.")]
        protected IdeaPluginRepository()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdeaPluginRepository"/> class.
        /// </summary>
        /// <param name="categories">The categories in the repository.</param>
        public IdeaPluginRepository(List<IdeaPluginCategory>? categories = null)
        {
            this.Categories = categories ?? new List<IdeaPluginCategory>();
        }
    }
}
