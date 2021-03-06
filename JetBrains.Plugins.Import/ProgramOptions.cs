//
//  ProgramOptions.cs
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
using System.IO;
using CommandLine;
using CommandLine.Text;

namespace JetBrains.Plugins.Import
{
    /// <summary>
    /// Represents the command-line options that can be passed to the program.
    /// </summary>
    public class ProgramOptions
    {
        /// <summary>
        /// Gets the input folder where the repository data resides. Defaults to the working directory.
        /// </summary>
        [Option
        (
            'i',
            "input",
            HelpText = "The input folder where the repository data resides. Defaults to the working directory."
        )]
        public string InputFolder { get; }

        /// <summary>
        /// Gets the path to the authentication file used for connecting to the database.
        /// </summary>
        [Option
        (
            'k',
            "keys",
            HelpText = "The path to the authentication file used for connecting to the database.",
            Required = true
        )]
        public string AuthenticationFile { get; }

        /// <summary>
        /// Gets a value indicating whether extra informational messages should be printed to the console.
        /// </summary>
        [Option('v', "verbose", HelpText = "Print extra informational messages to the console.", Default = false)]
        public bool VerboseOutput { get; }

        /// <summary>
        /// Gets a listing of usage examples.
        /// </summary>
        [Usage(ApplicationAlias = "JetBrains.Plugins.Mirror")]
        public static List<Example> UsageExamples => new List<Example>
        {
            new Example
            (
                "Import the repository data in the given folder, using the given keys.",
                new ProgramOptions("/var/www/jetbrains", "keys/auth.db", false)
            )
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgramOptions"/> class.
        /// </summary>
        /// <param name="inputFolder">The output folder.</param>
        /// <param name="authenticationFile">The authentication file path.</param>
        /// <param name="verboseOutput">Whether to enable verbose output.</param>
        public ProgramOptions
        (
            string? inputFolder,
            string authenticationFile,
            bool verboseOutput
        )
        {
            this.InputFolder = inputFolder ?? Directory.GetCurrentDirectory();
            this.VerboseOutput = verboseOutput;
            this.AuthenticationFile = authenticationFile;
        }
    }
}
