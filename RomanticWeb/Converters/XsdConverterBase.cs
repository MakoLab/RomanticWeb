using System;
using System.Collections.Generic;
using System.Linq;
using RomanticWeb.Model;

namespace RomanticWeb.Converters
{
    /// <summary>
    /// A base class for converting XSD-typed literals
    /// </summary>
    public abstract class XsdConverterBase:ILiteralNodeConverter
    {
        /// <summary>
        /// Get the XSD datatypes, which this converter supports
        /// </summary>
        protected abstract IEnumerable<Uri> SupportedTypes { get; }

        public abstract object Convert(Node objectNode);

        /// <summary>
        /// Check if a converter can convert the given XSD datatype
        /// </summary>
        public bool CanConvert(Uri dataType)
        {
            return SupportedTypes.Contains(dataType,new AbsoluteUriComparer());
        }
    }
}