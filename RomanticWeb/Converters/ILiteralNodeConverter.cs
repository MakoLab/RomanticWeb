using System;
using RomanticWeb.Model;

namespace RomanticWeb.Converters
{
    /// <summary>Defines a contract for converting literal nodes.</summary>
    public interface ILiteralNodeConverter
    {
        /// <summary>Converts a node to a literal value.</summary>
        object Convert(Node objectNode);

        /// <summary>Check if a converter can convert the given RDF data type.</summary>
        bool CanConvert(Uri dataType);
    }
}