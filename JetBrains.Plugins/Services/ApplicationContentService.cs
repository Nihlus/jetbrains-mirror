//
//  ApplicationContentService.cs
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

using System.IO;
using JetBrains.Plugins.Models.PostgreSQL;

namespace JetBrains.Plugins.Services
{
    /// <summary>
    /// Serves local application-specific content.
    /// </summary>
    public class ApplicationContentService
    {
        /// <summary>
        /// Gets the database connection string.
        /// </summary>
        public string ConnectionString { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationContentService"/> class.
        /// </summary>
        public ApplicationContentService()
        {
            this.ConnectionString = LoadConnectionString();
        }

        private string LoadConnectionString()
        {
            const string filename = "db.auth";
            var connectionStringPath = Path.Combine("content", "app", filename);
            if (!File.Exists(connectionStringPath))
            {
                throw new FileNotFoundException("Could not find the database credentials.", connectionStringPath);
            }

            if (!Passfile.TryParse(File.ReadAllText(connectionStringPath), out var result))
            {
                throw new InvalidDataException("The credential file was of an invalid format.");
            }

            return result.ConnectionString;
        }
    }
}
