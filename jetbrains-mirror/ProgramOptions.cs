//
//  ProgramOptions.cs
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
using System.IO;
using CommandLine;
using CommandLine.Text;
using JetBrains.Annotations;

namespace JetBrains.Mirror
{
    /// <summary>
    /// Represents the command-line options that can be passed to the program.
    /// </summary>
    public class ProgramOptions
    {
        /// <summary>
        /// Gets the output folder where the repository should be created. Defaults to the working directory.
        /// </summary>
        [NotNull]
        [Option
        (
            'o',
            "output",
            HelpText = "The output folder where the repository should be created. Defaults to the working directory."
        )]
        public string OutputFolder { get; }

        /// <summary>
        /// Gets the product versions to mirror plugins for.
        /// </summary>
        [NotNull]
        [Option('b', "versions", Required = true, HelpText = "The product versions to mirror plugins for.")]
        public IEnumerable<string> ProductVersions { get; }

        /// <summary>
        /// Gets a value indicating whether to mirror all available plugin versions, instead of just the latest available version.
        /// </summary>
        [Option
        (
            'a',
            "mirror-all-versions",
            HelpText = "Mirror all available plugin versions, instead of just the latest available version.",
            Default = false
        )]
        public bool MirrorAllVersions { get; }

        /// <summary>
        /// Gets a value indicating whether extra informational messages should be printed to the console.
        /// </summary>
        [Option('v', "verbose", HelpText = "Print extra informational messages to the console.", Default = false)]
        public bool VerboseOutput { get; }

        /// <summary>
        /// Gets a listing of usage examples.
        /// </summary>
        [Usage(ApplicationAlias = "jetbrains-mirror")]
        [NotNull]
        public static List<Example> UsageExamples => new List<Example>
        {
            new Example
            (
                "Mirror the latest versions of all plugins compatible with Rider 2019.1",
                new ProgramOptions(null, new[] { "RD-191.7141.355" }, false, false)
            ),
            new Example
            (
                "Mirror into a specific directory",
                new ProgramOptions("/var/www/jetbrains-mirror", new[] { "RD-191.7141.355" }, false, false)
            ),
            new Example
            (
                "Mirror multiple program versions",
                new ProgramOptions(null, new[] { "RD-191.7141.355", "CL-191.7479.33" }, false, false)
            )
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgramOptions"/> class.
        /// </summary>
        /// <param name="outputFolder">The output folder.</param>
        /// <param name="productVersions">The product versions to mirror for.</param>
        /// <param name="mirrorAllVersions">Whether to mirror all plugin versions.</param>
        /// <param name="verboseOutput">Whether to enable verbose output.</param>
        public ProgramOptions
        (
            [CanBeNull] string outputFolder,
            [CanBeNull] IEnumerable<string> productVersions,
            bool mirrorAllVersions,
            bool verboseOutput
        )
        {
            this.OutputFolder = outputFolder ?? Directory.GetCurrentDirectory();
            this.ProductVersions = productVersions ?? new List<string>();
            this.MirrorAllVersions = mirrorAllVersions;
            this.VerboseOutput = verboseOutput;
        }
    }
}
