//
//  Passfile.cs
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
using JetBrains.Annotations;

namespace JetBrains.Plugins.Models.PostgreSQL
{
    /// <summary>
    /// Represents a parsed PostgreSQL passfile.
    /// </summary>
    public class Passfile
    {
        /// <summary>
        /// Gets the host that the database server is hosted on.
        /// </summary>
        [NotNull]
        public string Host { get; }

        /// <summary>
        /// Gets the port that the database server listens on.
        /// </summary>
        public ushort Port { get; }

        /// <summary>
        /// Gets the name of the database to connect to.
        /// </summary>
        [NotNull]
        public string Database { get; }

        /// <summary>
        /// Gets the username to connect with.
        /// </summary>
        [NotNull]
        public string Username { get; }

        /// <summary>
        /// Gets the password to authenticate with.
        /// </summary>
        [NotNull]
        public string Password { get; }

        /// <summary>
        /// Gets the connection string that the passfile represents.
        /// </summary>
        [NotNull]
        public string ConnectionString { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Passfile"/> class.
        /// </summary>
        /// <param name="host">The database host.</param>
        /// <param name="port">The database port.</param>
        /// <param name="database">The database name.</param>
        /// <param name="username">The login username.</param>
        /// <param name="password">The login password.</param>
        public Passfile
        (
            [NotNull] string host,
            ushort port,
            [NotNull] string database,
            [NotNull] string username,
            [NotNull] string password
        )
        {
            this.Host = host ?? throw new ArgumentNullException(nameof(host));
            this.Port = port;
            this.Database = database ?? throw new ArgumentNullException(nameof(database));
            this.Username = username ?? throw new ArgumentNullException(nameof(username));
            this.Password = password ?? throw new ArgumentNullException(nameof(password));

            this.ConnectionString =
                $"Host={host};Port={port};Database={database};Username={username};Password={password}";
        }

        /// <summary>
        /// Parses a passfile structure from the given input string.
        /// </summary>
        /// <param name="content">The input string.</param>
        /// <param name="result">The resulting passfile.</param>
        /// <returns>true if the file was successfully parsed; otherwise, false.</returns>
        [Pure, ContractAnnotation("=> true, result : notnull; => false, result : null")]
        public static bool TryParse([NotNull] string content, [CanBeNull] out Passfile result)
        {
            content = content ?? throw new ArgumentNullException(nameof(content));
            result = null;

            var passfileContents = content.Split(':');
            if (passfileContents.Length != 5)
            {
                return false;
            }

            var host = passfileContents[0];
            if (string.IsNullOrEmpty(host))
            {
                return false;
            }

            if (!ushort.TryParse(passfileContents[1], out var port))
            {
                return false;
            }

            var database = passfileContents[2];
            if (string.IsNullOrEmpty(database))
            {
                return false;
            }

            var username = passfileContents[3];
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }

            var password = passfileContents[4];
            if (string.IsNullOrEmpty(password))
            {
                return false;
            }

            result = new Passfile(host, port, database, username, password);
            return true;
        }
    }
}
