using System;
using System.Collections.Generic;
using System.Xml;
using RomanticWeb.Model;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Converters
{
    /// <summary>
    /// Convert XSD floating point numbers to doubles
    /// </summary>
    public class DoubleConverter:XsdConverterBase
    {
        /// <summary>
        /// Gets Uris of xsd floating point types
        /// </summary>
        protected override IEnumerable<Uri> SupportedDataTypes
        {
            get
            {
                yield return Xsd.Float;
                yield return Xsd.Double;
            }
        }

        /// <inheritdoc/>
        public override Node ConvertBack(object value)
        {
            if (value is float)
            {
                return Node.ForLiteral(XmlConvert.ToString((float)value),Xsd.Float);
            }
            
            return Node.ForLiteral(XmlConvert.ToString((double)value),Xsd.Double);
        }

        /// <inheritdoc/>
        protected override object ConvertInternal(Node literalNode)
        {
            if (literalNode.Literal=="+INF")
            {
                return double.PositiveInfinity;
            }

            return XmlConvert.ToDouble(literalNode.Literal);
        }
    }
}