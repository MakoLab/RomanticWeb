using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml;
using RomanticWeb.Model;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Converters
{
    [Export(typeof(ILiteralNodeConverter))]
    public class DateTimeConverter:XsdConverterBase
    {
        protected override IEnumerable<Uri> SupportedTypes
        {
            get
            {
                yield return Xsd.DateTime;
                yield return Xsd.Date;
            }
        }

        public override object Convert(Node objectNode)
        {
            var dateTime=XmlConvert.ToDateTime(objectNode.Literal,XmlDateTimeSerializationMode.RoundtripKind);

            if (dateTime.Kind==DateTimeKind.Local)
            {
                return dateTime.ToUniversalTime();
            }

            return dateTime;
        }
    }
}