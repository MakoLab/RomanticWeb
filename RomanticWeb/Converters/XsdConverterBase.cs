using System;
using System.Collections.Generic;
using System.Linq;

namespace RomanticWeb.Converters
{
    public abstract class XsdConverterBase:ILiteralNodeConverter
    {
        protected abstract IEnumerable<Uri> SupportedTypes { get; }

        public abstract object Convert(Node objectNode);

        public bool CanConvert(Uri dataType)
        {
            return SupportedTypes.Contains(dataType,new AbsoluteUriComparer());
        }
    }
}