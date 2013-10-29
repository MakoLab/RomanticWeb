using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using RomanticWeb.Model;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Converters
{
    /// <summary>
    /// Converts XSD numeric types to numbers
    /// </summary>
    [Export(typeof(ILiteralNodeConverter))]
    public class IntegerConverter:XsdConverterBase
    {
        /// <summary>
        /// Gets xsd integral numeric datatypes
        /// </summary>
        protected override IEnumerable<Uri> SupportedTypes
        {
            get
            {
                yield return Xsd.Integer;
                yield return Xsd.Int;
                yield return Xsd.Long;
                yield return Xsd.Short;
                yield return Xsd.Byte;
                yield return Xsd.NonNegativeInteger;
                yield return Xsd.NonPositiveInteger;
                yield return Xsd.NegativeInteger;
                yield return Xsd.PositiveInteger;
                yield return Xsd.UnsignedByte;
                yield return Xsd.UnsignedInt;
                yield return Xsd.UnsignedLong;
                yield return Xsd.UnsignedShort;
            }
        }

        public override object Convert(Node objectNode)
        {
            return long.Parse(objectNode.Literal);
        }
    }
}