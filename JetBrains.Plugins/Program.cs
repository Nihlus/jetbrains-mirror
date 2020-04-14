//
//  Program.cs
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

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace JetBrains.Plugins
{
    /// <summary>
    /// The main class of the program.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point of the program.
        /// </summary>
        /// <param name="args">The arguments passed to the application.</param>
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Creates a webhost builder for the application.
        /// </summary>
        /// <param name="args">The arguments passed to the application.</param>
        /// <returns>The builder.</returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseKestrel(o =>
                {
                    o.ListenAnyIP(5000);
                });
    }
}
