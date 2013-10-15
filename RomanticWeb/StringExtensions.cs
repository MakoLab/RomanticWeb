using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static string Indent(this string text,int indentation)
        {
            string result=null;
            if (text!=null)
            {
                result=text;
                if (indentation>0)
                {
                    string indent=new String('\t',indentation);
                    result=indent+result.Replace("\r",System.String.Empty).Replace("\n",System.Environment.NewLine+indent);
                }
            }

            return result;
        }
    }
}