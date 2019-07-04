//
//  ResultBase.cs
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
using System.Globalization;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace JetBrains.Mirror.Results
{
    /// <summary>
    /// Serves as the base class for results.
    /// </summary>
    /// <typeparam name="TResultType">The actual type of the result.</typeparam>
    /// <typeparam name="TErrorType">The type of the error enumeration.</typeparam>
    public abstract class ResultBase<TResultType, TErrorType> : IResult<TErrorType>
        where TResultType : ResultBase<TResultType, TErrorType>
        where TErrorType : struct, Enum
    {
        /// <summary>
        /// Holds the enum value that's used for exceptions. If no appropriately named enum member is found, this field
        /// holds the default value.
        /// </summary>
        private static readonly TErrorType ExceptionErrorValue;

        /// <summary>
        /// Holds the actual error reason.
        /// </summary>
        [CanBeNull]
        private readonly string _errorReason;

        /// <inheritdoc />
        public TErrorType? Error { get; }

        /// <inheritdoc />
        [NotNull]
        public string ErrorReason
        {
            get
            {
                if (this.IsSuccess || _errorReason is null)
                {
                    throw new InvalidOperationException("The result does not contain a valid error.");
                }

                return _errorReason;
            }
        }

        /// <inheritdoc />
        public bool IsSuccess => !this.Error.HasValue;

        /// <summary>
        /// Gets the exception that caused the error, if any.
        /// </summary>
        public Exception Exception { get; }

        static ResultBase()
        {
            var availableErrors = typeof(TErrorType).GetEnumValues();
            foreach (var availableError in availableErrors.Cast<TErrorType>())
            {
                if (availableError.ToString().Equals("Exception", StringComparison.OrdinalIgnoreCase))
                {
                    ExceptionErrorValue = availableError;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultBase{TResultType, TErrorType}"/> class.
        /// </summary>
        protected ResultBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultBase{TResultType, TErrorType}"/> class.
        /// </summary>
        /// <param name="error">The error (if any).</param>
        /// <param name="errorReason">A more detailed error description.</param>
        /// <param name="exception">The exception that caused the error (if any).</param>
        protected ResultBase
        (
            [CanBeNull] TErrorType? error,
            [CanBeNull] string errorReason,
            [CanBeNull] Exception exception = null
        )
        {
            _errorReason = errorReason;

            this.Error = error;
            this.Exception = exception;
        }

        /// <summary>
        /// Creates a failed result based on another result.
        /// </summary>
        /// <param name="result">The result to base this result off of.</param>
        /// <returns>A failed result.</returns>
        [Pure]
        public static TResultType FromError([NotNull] IResult<TErrorType> result)
        {
            if (result.IsSuccess)
            {
                throw new InvalidOperationException("The original result was successful.");
            }

            // ReSharper disable once PossibleInvalidOperationException
            // ReSharper disable once AssignNullToNotNullAttribute
            return FromError(result.Error.Value, result.ErrorReason);
        }

        /// <summary>
        /// Creates a failed result based on an exception.
        /// </summary>
        /// <param name="exception">The exception to base this result off of.</param>
        /// <returns>A failed result.</returns>
        [Pure]
        public static TResultType FromError([NotNull] Exception exception)
        {
            return FromError(ExceptionErrorValue, exception.Message, exception);
        }

        /// <summary>
        /// Creates a failed result based on an exception.
        /// </summary>
        /// <param name="exception">The exception to base this result off of.</param>
        /// <param name="reason">The reason for the exception.</param>
        /// <returns>A failed result.</returns>
        [Pure]
        public static TResultType FromError([NotNull] Exception exception, [NotNull] string reason)
        {
            return FromError(ExceptionErrorValue, reason, exception);
        }

        /// <summary>
        /// Creates a failed result.
        /// </summary>
        /// <param name="error">The error that caused the failure.</param>
        /// <param name="reason">A more detailed error reason.</param>
        /// <param name="exception">The exception that caused the failure, if any.</param>
        /// <returns>A failed result.</returns>
        [Pure]
        public static TResultType FromError
        (
            TErrorType error,
            [NotNull] string reason,
            [CanBeNull] Exception exception = null
        )
        {
            return (TResultType)Activator.CreateInstance
            (
                typeof(TResultType),
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy,
                null,
                new object[] { error,  reason, exception },
                CultureInfo.InvariantCulture
            );
        }

        /// <inheritdoc />
        [NotNull]
        public override string ToString()
        {
            return this.IsSuccess ? "Success" : $"{this.Error}: {this.ErrorReason}";
        }
    }
}
