//
//  Endpoints.cs
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

using System.ComponentModel.DataAnnotations;

namespace JetBrains.Mirror.API
{
    /// <summary>
    /// Holds static data about endpoints in the JetBrains API.
    /// </summary>
    public static class Endpoints
    {
        /// <summary>
        /// Gets the base URL of the API.
        /// </summary>
        public const string BaseURL = "plugins.jetbrains.com";

        /// <summary>
        /// Holds static data about plugin listing endpoints.
        /// </summary>
        public static class PluginList
        {
            /// <summary>
            /// Gets the base path of the API on the URL.
            /// </summary>
            public const string BasePath = "plugins/list";

            /// <summary>
            /// Holds static parameter information about the <see cref="PluginList"/> endpoint.
            /// </summary>
            public static class Parameters
            {
                /// <summary>
                /// Gets the name of the build parameter. Using this parameter lists the latest versions of all plugins
                /// compatible with the given build.
                /// </summary>
                public const string Build = "build";

                /// <summary>
                /// Gets the name of the pluginId parameter. Using this parameter lists all uploaded versions of the
                /// specified plugin.
                /// </summary>
                public const string PluginID = "pluginId";
            }
        }

        /// <summary>
        /// Holds static data about plugin download endpoints.
        /// </summary>
        public static class PluginDownload
        {
            /// <summary>
            /// Gets the base path of the API on the URL.
            /// </summary>
            public const string BasePath = "plugin/download";

            /// <summary>
            /// Holds static parameter information about the <see cref="PluginDownload"/> endpoint.
            /// </summary>
            public static class Parameters
            {
                /// <summary>
                /// Gets the name of the id parameter. This specifies which plugin you want to download, and refers to
                /// the id taken from the plugin list (or specified in the plugin itself).
                /// This parameter is always required.
                /// </summary>
                [Required]
                public const string PluginID = "pluginId";

                /// <summary>
                /// Gets the name of the version parameter. Using this parameter downloads the specified version of the
                /// plugin.
                /// </summary>
                public const string Version = "version";
            }
        }

        /// <summary>
        /// Holds static data about plugin download endpoints.
        /// </summary>
        public static class PluginManager
        {
            /// <summary>
            /// Gets the base path of the API on the URL.
            /// </summary>
            public const string BasePath = "pluginManager";

            /// <summary>
            /// Holds static parameter information about the <see cref="PluginManager"/> endpoint.
            /// </summary>
            public static class Parameters
            {
                /// <summary>
                /// Gets the name of the action parameter. This parameter sets the mode of the plugin manager;
                /// typically, this is set to "download".
                /// </summary>
                [Required]
                public const string Action = "action";

                /// <summary>
                /// Gets the name of the id parameter. This specifies which plugin you want to download, and refers to
                /// the id taken from the plugin list (or specified in the plugin itself).
                /// This parameter is always required.
                /// </summary>
                [Required]
                public const string ID = "id";

                /// <summary>
                /// Gets the name of the build parameter. Using this parameter downloads the latest compatible version
                /// of the plugin for the given build.
                /// </summary>
                public const string Build = "build";
            }
        }
    }
}
