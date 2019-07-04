//
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
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Humanizer;
using JetBrains.Plugins.Mirror.API;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using RateLimiter;

namespace JetBrains.Plugins.Mirror
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
                .Or<TimeoutException>()
                .Or<TaskCanceledException>(tex => !tex.CancellationToken.IsCancellationRequested)
                .Or<IOException>(iex => iex.Message.Contains("Connection reset by peer"))
                .WaitAndRetryAsync
                (
                    6,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) +
                                    TimeSpan.FromMilliseconds(jitterer.Next(0, 100))
                );

            using (var services = new ServiceCollection()
                .AddHttpClient<JetbrainsPlugins, JetbrainsPlugins>()
                .ConfigurePrimaryHttpMessageHandler
                (
                    () =>
                        new SocketsHttpHandler
                        {
                            AllowAutoRedirect = true,
                        }
                )
                .ConfigureHttpClient
                (
                    client =>
                    {
                        // Timeouts are handled in the handler
                        client.Timeout = Timeout.InfiniteTimeSpan;

                        client.DefaultRequestHeaders.UserAgent.Clear();
                        client.DefaultRequestHeaders.UserAgent.ParseAdd("JetBrains Rider/191.7141355");
                    }
                )
                .ConfigureHttpMessageHandlerBuilder
                (
                    builder =>
                    {
                        var rateLimitingHandler = new RateLimitingHttpHandler
                        (
                            TimeLimiter.GetFromMaxCountByInterval(128, TimeSpan.FromSeconds(1))
                        );

                        var timeoutHandler = new TimeoutHttpHandler();

                        builder.AdditionalHandlers.Add(rateLimitingHandler);
                        builder.AdditionalHandlers.Add(timeoutHandler);
                    }
                )
                .AddPolicyHandler(retryPolicy)
                .Services
                .AddSingleton<RepositoryMirrorer>()
                .BuildServiceProvider())
            {
                var mirrorer = services.GetRequiredService<RepositoryMirrorer>();
                using (var cancellationSource = new CancellationTokenSource())
                {
                    await Console.Out.WriteLineAsync($"Fetching latest plugin versions for {Options.ProductVersions.Humanize()}...");
                    await mirrorer.MirrorRepositoriesAsync(Options.ProductVersions, cancellationSource.Token);
                }
            }
        }
    }
}
