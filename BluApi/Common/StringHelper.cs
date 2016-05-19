using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace BluApi.Common
{
    /// <summary>
    /// String helper class used by Chef API
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// Encode to Base 64
        /// </summary>
        /// <param name="input">input as string</param>
        /// <returns>output as base 64 encoded string</returns>
        public static string ToBase64EncodedSha1String(this string input)
        {
            return Convert.ToBase64String(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(input)));
        }

        /// <summary>
        /// String splitter
        /// </summary>
        /// <param name="input">input as string</param>
        /// <param name="length">length of split as int</param>
        /// <returns>IEnumerable string</returns>
        public static IEnumerable<string> Split(this string input, int length)
        {
            for (int i = 0; i < input.Length; i += length)
                yield return input.Substring(i, Math.Min(length, input.Length - i));
        }

        /// <summary>
        /// Returns string between two strings
        /// </summary>
        /// <param name="source">Source string</param>
        /// <param name="start">Start pattern</param>
        /// <param name="end">End pattern</param>
        /// <returns></returns>
        public static string StringBetween(this string source, string start, string end)
        {
            int from = source.IndexOf(start, StringComparison.Ordinal) + start.Length;
            int to = source.LastIndexOf(end, StringComparison.Ordinal);
            return source.Substring(from, to - from);
        }

        /// <summary>
        /// Return trimmed string from quotes and double quotes
        /// </summary>
        /// <param name="source">Source string</param>
        /// <returns>trimmed string</returns>
        public static string NoQuote(this string source)
        {
            return source.Trim('"').Trim('\'');
        }

        /// <summary>
        /// Return trimmed string, double quoted
        /// </summary>
        /// <param name="source">Source string</param>
        /// <returns>trimmed string, double quoted</returns>
        public static string DoubleQuote(this string source)
        {
            return "\"" + source.Trim('"').Trim('\'') + "\"";
        }

        /// <summary>
        /// Return trimmed string, single quoted
        /// </summary>
        /// <param name="source">Source string</param>
        /// <returns>trimmed string, single quoted</returns>
        public static string SingleQuote(this string source)
        {
            return "\'" + source.Trim('"').Trim('\'') + "\'";
        }
    }
}
