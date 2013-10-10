using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Converters
{
    [Export(typeof(ILiteralNodeConverter))]
    public class BooleanConverter:XsdConverterBase
    {
        protected override IEnumerable<Uri> SupportedTypes
        {
            get
            {
                yield return Xsd.Boolean;
            }
        }

        public override object Convert(Node objectNode)
        {
            return XmlConvert.ToBoolean(objectNode.Literal);
        }
    }
}