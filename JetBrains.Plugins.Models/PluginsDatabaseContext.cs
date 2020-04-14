//
//  PluginsDatabaseContext.cs
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

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace JetBrains.Plugins.Models
{
    /// <summary>
    /// Represents the plugin database.
    /// </summary>
    [PublicAPI]
    public class PluginsDatabaseContext : DbContext
    {
        /// <summary>
        /// Gets or sets the plugins in the database.
        /// </summary>
        public DbSet<Plugin> Plugins { get; set; } = null!;

        /// <summary>
        /// Gets or sets the categories in the database.
        /// </summary>
        public DbSet<PluginCategory> Categories { get; set; } = null!;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginsDatabaseContext"/> class.
        /// </summary>
        /// <param name="options">The options to use.</param>
        public PluginsDatabaseContext(DbContextOptions<PluginsDatabaseContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Configures the given options builder with the context's default expected options.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>The configured builder.</returns>
        public static DbContextOptionsBuilder ConfigureDefaultOptions
        (
            DbContextOptionsBuilder builder
        )
        {
            return builder.UseLazyLoadingProxies();
        }
    }
}
