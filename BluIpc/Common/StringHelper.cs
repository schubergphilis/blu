using System;
using System.Text.RegularExpressions;

namespace BluIpc.Common
{
    public static class StringExt
    {
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        public static string StringBetween(this string source, string start, string end)
        {
            int from = source.IndexOf(start, StringComparison.Ordinal) + start.Length;
            int to = source.IndexOf(end, StringComparison.Ordinal);
            return source.Substring(from, to - from);
        }

        public static string FormatForEventLog(this string value)
        {
            return
                Environment.NewLine +
                "------------------------------------------" +
                Environment.NewLine +
                value.TrimEnd(Environment.NewLine.ToCharArray()) +
                Environment.NewLine +
                "------------------------------------------" +
                Environment.NewLine;
        }

        public static string SingleQuote(this string source)
        {
            return "\'" + source.Trim('"').Trim('\'') + "\'";
        }

        public static string TransformForBlu(this string value)
        {
            string result = String.Empty;

            // Handle blu_true, blu_false and blu_nil
            string booleanAndNil = value.Replace("blu_true", "$True")
                .Replace("blu_false", "$False")
                .Replace("blu_nil", "$Null");

            if (booleanAndNil.Contains("blu_array"))
            {
                // Handle blu_array
                string[] raw = Regex.Split(booleanAndNil, @"blu_array@");
                result += raw[0];
                for (int i = 1; i < raw.Length; i++)
                {
                    string begin = raw[i].Substring(0, raw[i].IndexOf('('));
                    string between = raw[i].StringBetween("(", ")");
                    string end = raw[i].Substring(raw[i].IndexOf(')') + 1);
                    string bluArray = "@(" + between + ")";
                    result += begin + bluArray + end;
                }
            }
            else
            {
                result = booleanAndNil;
            }
            return result;
        }
    }
}
