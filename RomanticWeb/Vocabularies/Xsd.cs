using System;

namespace RomanticWeb.Vocabularies
{
    /// <summary>The XSD vocabulary (http://www.w3.org/2001/XMLSchema#).</summary>
    public static class Xsd
    {
#pragma warning disable 1591
        public const string BaseUri = "http://www.w3.org/2001/XMLSchema#";

        public static readonly Uri Int = new Uri(BaseUri + "int");

        public static readonly Uri Float = new Uri(BaseUri + "float");

        public static readonly Uri Double = new Uri(BaseUri + "double");

        public static readonly Uri Decimal = new Uri(BaseUri + "decimal");

        public static readonly Uri Integer = new Uri(BaseUri + "integer");

        public static readonly Uri Long = new Uri(BaseUri + "long");

        public static readonly Uri Byte = new Uri(BaseUri + "byte");

        public static readonly Uri Short = new Uri(BaseUri + "short");

        public static readonly Uri DateTime = new Uri(BaseUri + "dateTime");

        public static readonly Uri Date = new Uri(BaseUri + "date");

        public static readonly Uri Boolean = new Uri(BaseUri + "boolean");

        public static readonly Uri Time = new Uri(BaseUri + "time");

        public static readonly Uri UnsignedInt = new Uri(BaseUri + "unsignedInt");

        public static readonly Uri UnsignedByte = new Uri(BaseUri + "unsignedByte");

        public static readonly Uri UnsignedLong = new Uri(BaseUri + "unsignedLong");

        public static readonly Uri UnsignedShort = new Uri(BaseUri + "unsignedShort");

        public static readonly Uri NegativeInteger = new Uri(BaseUri + "unsignedInteger");

        public static readonly Uri PositiveInteger = new Uri(BaseUri + "positiveInteger");

        public static readonly Uri NonNegativeInteger = new Uri(BaseUri + "nonNegativeInteger");

        public static readonly Uri NonPositiveInteger = new Uri(BaseUri + "nonPositiveInteger");

        public static readonly Uri Duration = new Uri(BaseUri + "duration");

        public static readonly Uri AnyUri = new Uri(BaseUri + "anyUri");

        public static readonly Uri String = new Uri(BaseUri + "string");

        public static readonly Uri Base64Binary = new Uri(BaseUri + "base64Binary");

        public static readonly Uri NCName = new Uri(BaseUri + "NCName");
#pragma warning restore 1591
    }
}