using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml;
using RomanticWeb.Model;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Converters
{
    [Export(typeof(ILiteralNodeConverter))]
    public class DecimalConverter : XsdConverterBase
    {
        protected override IEnumerable<Uri> SupportedTypes
        {
            get
            {
                yield return Xsd.Decimal;
            }
        }
        
        public override object Convert(Node objectNode)
        {
            return XmlConvert.ToDecimal(objectNode.Literal);
        }
    }
}