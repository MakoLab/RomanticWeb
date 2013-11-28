using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml;
using RomanticWeb.Model;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Converters
{
    /// <summary>
    /// Converts xsd:duration to <see cref="TimeSpan"/>
    /// </summary>
    [Export(typeof(ILiteralNodeConverter))]
    public class DurationConverter:XsdConverterBase
    {
        /// <summary>
        /// Gets Uri of xsd:duration
        /// </summary>
        protected override IEnumerable<Uri> SupportedTypes
        {
            get
            {
                yield return Xsd.Duration;
            }
        }

        /// <summary>
        /// Converts xsd:duration to <see cref="Duration"/>
        /// </summary>
        public override object Convert(Node objectNode)
        {
            return Duration.Parse(objectNode.Literal);
        }
    }
}