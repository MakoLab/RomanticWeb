using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml;
using RomanticWeb.Model;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Converters
{
    /// <summary>
    /// Converter for xsd:boolean
    /// </summary>
    [Export(typeof(ILiteralNodeConverter))]
    public class BooleanConverter:XsdConverterBase
    {
        /// <summary>
        /// Gets Uri of xsd:boolean
        /// </summary>
        protected override IEnumerable<Uri> SupportedTypes
        {
            get
            {
                yield return Xsd.Boolean;
            }
        }

        /// <summary>
        /// Converts xsd:boolean to <see cref="bool"/>
        /// </summary>
        public override object Convert(Node objectNode)
        {
            return XmlConvert.ToBoolean(objectNode.Literal);
        }
    }
}