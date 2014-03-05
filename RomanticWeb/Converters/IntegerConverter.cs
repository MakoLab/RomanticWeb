using System;
using System.Collections.Generic;
using RomanticWeb.Model;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Converters
{
    /// <summary>
    /// Converts XSD numeric types to numbers
    /// </summary>
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

        /// <summary>
        /// Converts xsd:int (and subtypes) into <see cref="long"/>
        /// </summary>
        public override object Convert(Node objectNode)
        {
            switch ((objectNode.DataType!=null?objectNode.DataType.AbsoluteUri:null))
            {
                case Xsd.BaseUri+"unsignedInt":
                    return UInt32.Parse(objectNode.Literal);
                case Xsd.BaseUri+"int":
                    return Int32.Parse(objectNode.Literal);
                case Xsd.BaseUri+"unsignedShort":
                    return UInt16.Parse(objectNode.Literal);
                case Xsd.BaseUri+"short":
                    return Int16.Parse(objectNode.Literal);
                case Xsd.BaseUri+"unsignedByte":
                    return Byte.Parse(objectNode.Literal);
                case Xsd.BaseUri+"byte":
                    return SByte.Parse(objectNode.Literal);
                case Xsd.BaseUri+"long":
                case Xsd.BaseUri+"integer":
                case Xsd.BaseUri+"nonPositiveInteger":
                case Xsd.BaseUri+"negativeInteger":
                default:
                    return Int64.Parse(objectNode.Literal);
                case Xsd.BaseUri+"unsignedLong":
                case Xsd.BaseUri+"nonNegativeInteger":
                case Xsd.BaseUri+"positiveInteger":
                    return UInt64.Parse(objectNode.Literal);
            }
        }
    }
}