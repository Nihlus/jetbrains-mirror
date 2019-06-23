﻿//
//  Program.cs
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
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Humanizer;
using Humanizer.Bytes;
using JetBrains.Mirror.API;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace JetBrains.Mirror
{
    /// <summary>
    /// Represents the main class of the program.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Gets the command-line options that were passed to the program.
        /// </summary>
        public static ProgramOptions Options { get; private set; }

        private static async Task Main(string[] args)
        {
            var parsingResult = Parser.Default.ParseArguments<ProgramOptions>(args);
            if (parsingResult is Parsed<ProgramOptions> success)
            {
                Options = success.Value;
            }
            else
            {
                return;
            }

            var jitterer = new Random();
            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TaskCanceledException>(tex => !tex.CancellationToken.IsCancellationRequested)
                .WaitAndRetryAsync
                (
                    3,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) +
                                    TimeSpan.FromMilliseconds(jitterer.Next(0, 100))
                );

            var services = new ServiceCollection()
                .AddHttpClient<JetbrainsPlugins, JetbrainsPlugins>()
                .ConfigurePrimaryHttpMessageHandler
                (
                    () =>
                        new SocketsHttpHandler
                        {
                            AllowAutoRedirect = true
                        }
                )
                .AddPolicyHandler(retryPolicy)
                .Services
                .AddSingleton<RepositoryMirrorer>()
                .BuildServiceProvider();

            var mirrorer = services.GetRequiredService<RepositoryMirrorer>();
            using (var cancellationSource = new CancellationTokenSource())
            {
                await Console.Out.WriteLineAsync($"Fetching latest plugin versions for {Options.ProductVersions.Humanize()}...");
                await mirrorer.MirrorRepositoriesAsync(Options.ProductVersions, cancellationSource.Token);
            }
        }
    }
}
