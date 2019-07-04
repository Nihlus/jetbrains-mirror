//
//  PluginDependency.cs
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

namespace JetBrains.Plugins.Models
{
    /// <summary>
    /// Represents a dependency on another plugin.
    /// </summary>
    public class PluginDependency : EFEntity
    {
        /// <summary>
        /// Gets or sets the plugin that depends on the dependency.
        /// </summary>
        public Plugin Depender { get; set; }

        /// <summary>
        /// Gets or sets the dependency of the depender.
        /// </summary>
        public Plugin Dependency { get; set; }
    }
}
