using System;
using System.Collections.Generic;
using System.Xml;
using RomanticWeb.Model;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Converters
{
    /// <summary>
    /// Converter for xsd:boolean
    /// </summary>
    public class BooleanConverter:XsdConverterBase
    {
        /// <summary>
        /// Gets Uri of xsd:boolean
        /// </summary>
        protected override IEnumerable<Uri> SupportedDataTypes
        {
            get
            {
                yield return Xsd.Boolean;
            }
        }

        /// <summary>
        /// Converts a bool value to it's XML string representation
        /// </summary>
        public override Node ConvertBack(object value)
        {
            return Node.ForLiteral(XmlConvert.ToString((bool)value),Xsd.Boolean);
        }

        /// <summary>
        /// Converts xsd:boolean to <see cref="bool"/>
        /// </summary>
        protected override object ConvertInternal(Node objectNode)
        {
            return XmlConvert.ToBoolean(objectNode.Literal);
        }
    }
}