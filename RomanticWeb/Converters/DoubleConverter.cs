using System;
using System.Collections.Generic;
using System.Xml;
using NullGuard;
using RomanticWeb.Model;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Converters
{
    /// <summary>Convert XSD floating point numbers to doubles.</summary>
    public class DoubleConverter : XsdConverterBase
    {
        /// <summary>Gets Uris of xsd floating point types.</summary>
        protected override IEnumerable<Uri> SupportedDataTypes
        {
            get
            {
                yield return Xsd.Float;
                yield return Xsd.Double;
            }
        }

        /// <inheritdoc/>
        public override Node ConvertBack(object value, IEntityContext context)
        {
            if (value is float)
            {
                return Node.ForLiteral(XmlConvert.ToString((float)value), Xsd.Float);
            }

            return Node.ForLiteral(XmlConvert.ToString((double)value), Xsd.Double);
        }

        /// <inheritdoc />
        [return: AllowNull]
        public override Uri CanConvertBack(Type type)
        {
            switch (type.FullName)
            {
                case "System.Single":
                    return Xsd.Float;
                case "System.Double":
                    return Xsd.Double;
                default:
                    return null;
            }
        }

        /// <inheritdoc/>
        protected override object ConvertInternal(Node literalNode)
        {
            if (literalNode.Literal == "+INF")
            {
                return (AbsoluteUriComparer.Default.Equals(literalNode.DataType, Xsd.Float) ?
                    (object)float.PositiveInfinity :
                    double.PositiveInfinity);
            }

            return (AbsoluteUriComparer.Default.Equals(literalNode.DataType, Xsd.Float) ?
                (object)XmlConvert.ToSingle(literalNode.Literal) :
                XmlConvert.ToDouble(literalNode.Literal));
        }
    }
}