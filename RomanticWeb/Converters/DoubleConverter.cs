using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml;
using RomanticWeb.Model;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Converters
{
    [Export(typeof(ILiteralNodeConverter))]
    public class DoubleConverter:XsdConverterBase
    {
        protected override IEnumerable<Uri> SupportedTypes
        {
            get
            {
                yield return Xsd.Float;
                yield return Xsd.Double;
            }
        }

        public override object Convert(Node objectNode)
        {
            if (objectNode.Literal=="+INF")
            {
                return double.PositiveInfinity;
            }
            
            return XmlConvert.ToDouble(objectNode.Literal);
        }
    }
}