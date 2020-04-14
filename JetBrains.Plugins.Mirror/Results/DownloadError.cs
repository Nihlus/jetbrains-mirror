//
//  DownloadError.cs
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

namespace JetBrains.Plugins.Mirror.Results
{
    /// <summary>
    /// Enumerates various error states.
    /// </summary>
    public enum DownloadError
    {
        /// <summary>
        /// The result failed because of an uncaught exception.
        /// </summary>
        Exception,

        /// <summary>
        /// The result failed because the request timed out.
        /// </summary>
        Timeout,

        /// <summary>
        /// The result failed because the server returned an invalid response.
        /// </summary>
        InvalidResponse,

        /// <summary>
        /// The result failed, but we don't know why.
        /// </summary>
        Unknown
    }
}
