using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml;
using RomanticWeb.Model;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Converters
{
    [Export(typeof(ILiteralNodeConverter))]
    public class DurationConverter:XsdConverterBase
    {
        protected override IEnumerable<Uri> SupportedTypes
        {
            get
            {
                yield return Xsd.Duration;
            }
        }

        public override object Convert(Node objectNode)
        {
            return XmlConvert.ToTimeSpan(objectNode.Literal);
        }
    }
}