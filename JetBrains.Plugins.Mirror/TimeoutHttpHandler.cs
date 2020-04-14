//
//  TimeoutHttpHandler.cs
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

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace JetBrains.Plugins.Mirror
{
    /// <summary>
    /// Represents a http handler that has a configured timeout..
    /// </summary>
    public class TimeoutHttpHandler : DelegatingHandler
    {
        /// <inheritdoc />
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            using var cancellationSource = new CancellationTokenSource();
            var timeout = Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            var actualRequest = base.SendAsync(request, cancellationSource.Token);

            await Task.WhenAny(timeout, actualRequest);
            if (timeout.IsCompleted)
            {
                cancellationSource.Cancel();

                await timeout;
                throw new TimeoutException();
            }

            return await actualRequest;
        }
    }
}
