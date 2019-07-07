//
//  PluginCategoryExtensions.cs
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
using JetBrains.Annotations;
using JetBrains.Plugins.Models;
using PluginCategory = JetBrains.Plugins.Models.API.XML.PluginCategory;

namespace JetBrains.Plugins.Import.Extensions
{
    /// <summary>
    /// Extensions methods for the <see cref="PluginCategory"/> class.
    /// </summary>
    public static class PluginCategoryExtensions
    {
        /// <summary>
        /// Maps the given <see cref="PluginCategory"/> to a <see cref="Models.PluginCategory"/>.
        /// </summary>
        /// <param name="this">The category.</param>
        /// <returns>The mapped category.</returns>
        [NotNull]
        public static Models.PluginCategory ToEntity([NotNull] this PluginCategory @this)
        {
            return new Models.PluginCategory(@this.Name);
        }
    }
}
