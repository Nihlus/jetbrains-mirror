//
//  StringSlugger.cs
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

using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Jetbrains.Plugins.Helpers
{
    /// <summary>
    /// Slugs strings provided to it. Based on
    /// https://predicatet.blogspot.com/2009/04/improved-c-slug-generator-or-how-to.html.
    /// </summary>
    public static class StringSlugger
    {
        /// <summary>
        /// Generates a slug based on a provided phrase.
        /// </summary>
        /// <param name="phrase">The phrase.</param>
        /// <returns>The generated slug.</returns>
        [NotNull]
        public static string GenerateSlug([NotNull] this string phrase)
        {
            var str = phrase.RemoveDiacritics().ToLower();

            // Invalid chars
            str = Regex.Replace(str, @"[^a-z0-9\s-]", string.Empty);

            // Convert multiple spaces into one space
            str = Regex.Replace(str, @"\s+", " ").Trim();

            // Cut and trim
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
            str = Regex.Replace(str, @"\s", "-"); // hyphens

            return str;
        }

        /// <summary>
        /// Removes diacritics from the given string.
        /// </summary>
        /// <param name="value">The string.</param>
        /// <returns>The string, with diacritics removed.</returns>
        private static string RemoveDiacritics([NotNull] this string value)
        {
            var normalizedString = value.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
