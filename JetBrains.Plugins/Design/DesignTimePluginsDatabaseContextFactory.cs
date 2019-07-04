//
//  DesignTimePluginsDatabaseContextFactory.cs
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

using JetBrains.Annotations;
using JetBrains.Plugins.Models;
using JetBrains.Plugins.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace JetBrains.Plugins.Design
{
    /// <summary>
    /// Design-time factory for <see cref="PluginsDatabaseContext"/> instances.
    /// </summary>
    public class DesignTimePluginsDatabaseContextFactory : IDesignTimeDbContextFactory<PluginsDatabaseContext>
    {
        /// <inheritdoc/>
        [NotNull]
        public PluginsDatabaseContext CreateDbContext(string[] args)
        {
            var content = new ApplicationContentService();
            var options = new DbContextOptionsBuilder<PluginsDatabaseContext>()
                .UseNpgsql(content.ConnectionString)
                .Options;

            return new PluginsDatabaseContext(options);
        }
    }
}
