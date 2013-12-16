using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RomanticWeb.Model;

namespace RomanticWeb.Converters
{
    /// <summary>Converts Base64 literals.</summary>
    public class Base64BinaryConverter:ILiteralNodeConverter
    {
        /// <summary>Converts given Base64 binary literal into an array of bytes.</summary>
        /// <param name="objectNode">Node with Base64 binary literal.</param>
        /// <returns>Array of bytes or null if the passed node is also null.</returns>
        public object Convert(Node objectNode)
        {
            return (objectNode.Literal!=null?System.Convert.FromBase64String(objectNode.Literal):null);
        }

        /// <summary>Checks for ability to convert given data type.</summary>
        /// <param name="dataType">Data type to be converted</param>
        /// <returns><b>true</b> if the data type is Base64 binary; otherwise <b>false</b>.</returns>
        public bool CanConvert(Uri dataType)
        {
            return dataType==Vocabularies.Xsd.Base64Binary;
        }
    }
}