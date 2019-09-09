//
//  IdeaPluginExtensions.cs
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using JetBrains.Annotations;
using JetBrains.Plugins.Import.Helpers;
using JetBrains.Plugins.Models;
using JetBrains.Plugins.Models.API.XML;

namespace JetBrains.Plugins.Import.Extensions
{
    /// <summary>
    /// Extension methods for the <see cref="IdeaPlugin"/> class.
    /// </summary>
    public static class IdeaPluginExtensions
    {
        /// <summary>
        /// Maps the given <see cref="IdeaPlugin"/> to a <see cref="Plugin"/>.
        /// </summary>
        /// <param name="this">The IdeaPlugin.</param>
        /// <param name="dbCategory">The plugin category.</param>
        /// <returns>The mapped plugin.</returns>
        [NotNull]
        public static Plugin ToEntity
        (
            [NotNull] this IdeaPlugin @this,
            [NotNull] PluginCategory dbCategory
        )
        {
            var result = new Plugin
            (
                @this.Name,
                dbCategory,
                @this.ID,
                @this.Description,
                @this.Vendor.ToEntity()
            );

            result.Tags = @this.Tags?.Split(';').ToList() ?? new List<string>();
            result.Rating = @this.Rating;
            result.ProjectURL = @this.ProjectURL ?? string.Empty;

            return result;
        }

        /// <summary>
        /// Maps the given <see cref="IdeaPlugin"/> to a <see cref="PluginRelease"/>.
        /// </summary>
        /// <param name="this">The IdeaPlugin.</param>
        /// <param name="dbPlugin">The plugin that the release belongs to.</param>
        /// <returns>The mapped release.</returns>
        [NotNull]
        public static PluginRelease ToReleaseEntity
        (
            [NotNull] this IdeaPlugin @this,
            [NotNull] Plugin dbPlugin
        )
        {
            string SelectBestSinceValue(IdeaVersion version)
            {
                if (version.SinceBuild is null || version.SinceBuild == "n/a")
                {
                    return version.Min;
                }

                return version.SinceBuild;
            }

            string SelectBestUntilValue(IdeaVersion version)
            {
                if (version.UntilBuild is null || version.UntilBuild == "n/a")
                {
                    return version.Max;
                }

                return version.UntilBuild;
            }

            DateTime ParseDateFromMilliseconds(string value)
            {
                var millis = long.Parse(value);
                return DateTimeOffset.FromUnixTimeMilliseconds(millis).UtcDateTime;
            }

            var sinceBuildValue = SelectBestSinceValue(@this.IdeaVersion);
            IDEVersion sinceBuild = null;
            if (!(sinceBuildValue is null))
            {
                if (!IDEVersion.TryParse(sinceBuildValue, out sinceBuild))
                {
                    throw new InvalidDataException("Bad version string.");
                }
            }

            var untilBuildValue = SelectBestUntilValue(@this.IdeaVersion);
            IDEVersion untilBuild = null;
            if (!(untilBuildValue is null))
            {
                if (!IDEVersion.TryParse(untilBuildValue, out untilBuild))
                {
                    throw new InvalidDataException("Bad version string.");
                }
            }

            var versionRange = new IDEVersionRange
            {
                SinceBuild = sinceBuild ?? IDEVersion.Invalid,
                UntilBuild = untilBuild ?? IDEVersion.Invalid
            };

            // Get the file size and hash
            // TODO: refactor
            var basePath = Program.Options.InputFolder;
            var pluginFolder = Path.Combine
            (
                basePath,
                "plugins",
                dbPlugin.Category.Name.GenerateSlug(),
                dbPlugin.Name.GenerateSlug(),
                @this.Version
            );

            var pluginFile = Directory.EnumerateFiles(pluginFolder).FirstOrDefault();
            if (pluginFile is null)
            {
                throw new FileNotFoundException("Couldn't find the released plugin file. Missing data?");
            }

            string hash;
            using (var md5 = MD5.Create())
            {
                using (var file = File.OpenRead(pluginFile))
                {
                    var md5Sum = md5.ComputeHash(file);
                    hash = BitConverter.ToString(md5Sum).Replace("-", string.Empty).ToLowerInvariant();
                }
            }

            var fileInfo = new FileInfo(pluginFile);
            var size = fileInfo.Length;

            var result = new PluginRelease
            (
                dbPlugin,
                @this.ChangeNotes,
                size,
                ParseDateFromMilliseconds(@this.UploadDate),
                hash,
                @this.Version,
                versionRange,
                @this.Depends
            );

            result.Downloads = @this.Downloads;

            return result;
        }
    }
}
