//
//  IResult.cs
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

namespace JetBrains.Mirror.Results
{
    /// <summary>
    /// Represents the public API of a result.
    /// </summary>
    /// <typeparam name="TErrorType">The type of the error enumeration.</typeparam>
    public interface IResult<TErrorType> where TErrorType : struct, Enum
    {
        /// <summary>
        /// Gets a value indicating whether the result was successful.
        /// </summary>
        bool IsSuccess { get; }

        /// <summary>
        /// Gets the descriptive reason that the result failed.
        /// </summary>
        [CanBeNull]
        string ErrorReason { get; }

        /// <summary>
        /// Gets the reason the result failed.
        /// </summary>
        [CanBeNull]
        TErrorType? Error { get; }

        /// <summary>
        /// Gets the exception that caused the result to fail.
        /// </summary>
        [CanBeNull]
        Exception Exception { get; }
    }
}
