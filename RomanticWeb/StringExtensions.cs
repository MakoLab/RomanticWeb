using System;
using System.Text.RegularExpressions;

namespace RomanticWeb
{
    /// <summary>Provides useful string helper methods.</summary>
    public static class StringExtensions
    {
        /// <summary>Indents given string with given depth.</summary>
        /// <param name="text">Text to be indented.</param>
        /// <param name="indentation">Depth of the indentation.</param>
        /// <remarks>This method removes all occurances of a carriage return characters and replaces all occurances of a new line character with a environment specific new line string and indentation.</remarks>
        /// <returns>Text with indentation.</returns>
        public static string Indent(this string text, int indentation)
        {
            string result = null;
            if (text != null)
            {
                result = text;
                if (indentation > 0)
                {
                    string indent = new String('\t', indentation);
                    result = indent + result.Replace("\r", System.String.Empty).Replace("\n", System.Environment.NewLine + indent);
                }
            }

            return result;
        }

        /// <summary>Converts given text to camel case string.</summary>
        /// <param name="text">Input text.</param>
        /// <returns>Came case string or null.</returns>
        public static string CamelCase(this string text)
        {
            if (System.String.IsNullOrEmpty(text))
            {
                return text;
            }

            string result = text.PascalCase();
            return Char.ToLower(result[0]) + result.Substring(1);
        }

        /// <summary>Converts given text to pascal case string.</summary>
        /// <param name="text">Input text.</param>
        /// <returns>Pascal case string or null.</returns>
        public static string PascalCase(this string text)
        {
            if (System.String.IsNullOrEmpty(text))
            {
                return text;
            }

            return Regex.Replace(Regex.Replace(text, "[^a-zA-Z0-9_]", "_"), " [a-z]", match => match.Value.Substring(1).ToUpper());
        }
    }
}