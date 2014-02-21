using System;
using System.Collections.Generic;
using System.Xml;
using RomanticWeb.Model;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Converters
{
    /// <summary>
    /// Converter for xsd:decimal
    /// </summary>
    public class DecimalConverter : XsdConverterBase
    {
        /// <summary>
        /// Gets Uri of xsd:decimal
        /// </summary>
        protected override IEnumerable<Uri> SupportedTypes
        {
            get
            {
                yield return Xsd.Decimal;
            }
        }
        
        /// <summary>
        /// Converts xsd:decimal to <see cref="decimal"/>
        /// </summary>
        public override object Convert(Node objectNode)
        {
            return XmlConvert.ToDecimal(objectNode.Literal);
        }
    }
}