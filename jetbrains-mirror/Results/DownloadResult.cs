//
//  DownloadResult.cs
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
using JetBrains.Mirror.XML;

namespace JetBrains.Mirror.Results
{
    /// <summary>
    /// Represents the result of a download operation.
    /// </summary>
    public sealed class DownloadResult : ResultBase<DownloadResult, DownloadError>
    {
        /// <summary>
        /// Gets the plugin that was downloaded.
        /// </summary>
        [CanBeNull]
        public IdeaPlugin Plugin { get; }

        /// <summary>
        /// Gets the action that was performed by the operation.
        /// </summary>
        [CanBeNull]
        public DownloadAction? Action { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadResult"/> class.
        /// </summary>
        /// <param name="plugin">The plugin that was downloaded.</param>
        /// <param name="action">The action that was performed.</param>
        private DownloadResult(IdeaPlugin plugin, DownloadAction action)
        {
            this.Plugin = plugin;
            this.Action = action;
        }

        /// <inheritdoc cref="ResultBase{DownloadResult}(TErrorType?,string,Exception)"/>
        [UsedImplicitly]
        private DownloadResult
        (
            [CanBeNull] DownloadError? error,
            [CanBeNull] string errorReason,
            [CanBeNull] Exception exception = null
        )
            : base(error, errorReason, exception)
        {
        }

        /// <inheritdoc cref="ResultBase{DownloadResult}(TErrorType?,string,Exception)"/>
        private DownloadResult
        (
            [CanBeNull] IdeaPlugin plugin,
            [CanBeNull] DownloadError? error,
            [CanBeNull] string errorReason,
            [CanBeNull] Exception exception = null
        )
            : base(error, errorReason, exception)
        {
            this.Plugin = plugin;
        }

        /// <summary>
        /// Creates a successful result.
        /// </summary>
        /// <param name="plugin">The plugin that was successfully downloaded.</param>
        /// <param name="action">The action that was performed.</param>
        /// <returns>The result.</returns>
        [NotNull]
        public static DownloadResult FromSuccess(IdeaPlugin plugin, DownloadAction action)
        {
            return new DownloadResult(plugin, action);
        }

        /// <summary>
        /// Creates a failed result based on another result.
        /// </summary>
        /// <param name="plugin">The plugin that failed to download.</param>
        /// <param name="result">The result to base this result off of.</param>
        /// <returns>A failed result.</returns>
        [Pure]
        [NotNull]
        public static DownloadResult FromError(IdeaPlugin plugin, [NotNull] IResult<DownloadError> result)
        {
            if (result.IsSuccess)
            {
                throw new InvalidOperationException("The original result was successful.");
            }

            // ReSharper disable once PossibleInvalidOperationException
            // ReSharper disable once AssignNullToNotNullAttribute
            return FromError(plugin, result.Error.Value, result.ErrorReason);
        }

        /// <summary>
        /// Creates a failed result based on an exception.
        /// </summary>
        /// <param name="plugin">The plugin that failed to download.</param>
        /// <param name="exception">The exception to base this result off of.</param>
        /// <returns>A failed result.</returns>
        [Pure]
        [NotNull]
        public static DownloadResult FromError(IdeaPlugin plugin, [NotNull] Exception exception)
        {
            return FromError(plugin, DownloadError.Exception, exception.Message, exception);
        }

        /// <summary>
        /// Creates a failed result based on an exception.
        /// </summary>
        /// <param name="plugin">The plugin that failed to download.</param>
        /// <param name="exception">The exception to base this result off of.</param>
        /// <param name="reason">The reason for the exception.</param>
        /// <returns>A failed result.</returns>
        [Pure]
        [NotNull]
        public static DownloadResult FromError(IdeaPlugin plugin, [NotNull] Exception exception, [NotNull] string reason)
        {
            return FromError(plugin, DownloadError.Exception, reason, exception);
        }

        /// <summary>
        /// Creates a failed result.
        /// </summary>
        /// <param name="plugin">The plugin that failed to download.</param>
        /// <param name="error">The error that caused the failure.</param>
        /// <param name="reason">A more detailed error reason.</param>
        /// <param name="exception">The exception that caused the failure, if any.</param>
        /// <returns>A failed result.</returns>
        [Pure]
        [NotNull]
        public static DownloadResult FromError
        (
            IdeaPlugin plugin,
            DownloadError error,
            [NotNull] string reason,
            [CanBeNull] Exception exception = null
        )
        {
            return new DownloadResult(plugin, error, reason, exception);
        }
    }
}
