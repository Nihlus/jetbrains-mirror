//
//  RateLimitingHttpHandler.cs
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

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using RateLimiter;

namespace JetBrains.Plugins.Mirror
{
    /// <summary>
    /// Represents a http handler that has a configured rate limit.
    /// </summary>
    public class RateLimitingHttpHandler : DelegatingHandler
    {
        private readonly TimeLimiter _rateLimiter;

        /// <summary>
        /// Initializes a new instance of the <see cref="RateLimitingHttpHandler"/> class.
        /// </summary>
        /// <param name="rateLimiter">The rate limiter to use.</param>
        public RateLimitingHttpHandler(TimeLimiter rateLimiter)
        {
            _rateLimiter = rateLimiter;
        }

        /// <inheritdoc />
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return await _rateLimiter.Enqueue(() => base.SendAsync(request, cancellationToken), cancellationToken);
        }
    }
}
