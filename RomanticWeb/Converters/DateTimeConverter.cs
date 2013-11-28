using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml;
using RomanticWeb.Model;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Converters
{
    /// <summary>
    /// Converts xsd:date and xsd:datetime to <see cref="DateTime"/>
    /// </summary>
    [Export(typeof(ILiteralNodeConverter))]
    public class DateTimeConverter:XsdConverterBase
    {
        /// <summary>
        /// Gets xsd date datatypes
        /// </summary>
        protected override IEnumerable<Uri> SupportedTypes
        {
            get
            {
                yield return Xsd.Time;
                yield return Xsd.DateTime;
                yield return Xsd.Date;
            }
        }

        /// <summary>
        /// Converts xsd:date, xsd:time and xsd:dateTime into <see cref="DateTime"/>
        /// </summary>
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