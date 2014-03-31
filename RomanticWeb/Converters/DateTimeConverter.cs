using System;
using System.Collections.Generic;
using System.Xml;
using RomanticWeb.Model;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Converters
{
    /// <summary>
    /// Converts xsd:date and xsd:datetime to <see cref="DateTime"/>
    /// </summary>
    public class DateTimeConverter:XsdConverterBase
    {
        /// <summary>
        /// Gets xsd date datatypes
        /// </summary>
        protected override IEnumerable<Uri> SupportedDataTypes
        {
            get
            {
                yield return Xsd.Time;
                yield return Xsd.DateTime;
                yield return Xsd.Date;
            }
        }

        public override Node ConvertBack(object value)
        {
            // todo: xsd:Time and xsd:Date
            return Node.ForLiteral(XmlConvert.ToString((DateTime)value,XmlDateTimeSerializationMode.RoundtripKind),Xsd.DateTime); 
        }

        protected override object ConvertInternal(Node literalNode)
        {
            var dateTime = XmlConvert.ToDateTime(literalNode.Literal,XmlDateTimeSerializationMode.RoundtripKind);

            if (dateTime.Kind == DateTimeKind.Local)
            {
                return dateTime.ToUniversalTime();
            }

            return dateTime;
        }
    }
}