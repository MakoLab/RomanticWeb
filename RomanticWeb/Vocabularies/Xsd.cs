using System;

namespace RomanticWeb.Vocabularies
{
    internal static class Xsd
    {
        public const string BaseUri = "http://www.w3.org/2001/XMLSchema#";

        public static Uri Int
        {
            get
            {
                return new Uri(BaseUri + "int");
            }
        }

        public static Uri Float
        {
            get
            {
                return new Uri(BaseUri + "float");
            }
        }

        public static Uri Double
        {
            get
            {
                return new Uri(BaseUri + "double");
            }
        }

        public static Uri Decimal
        {
            get
            {
                return new Uri(BaseUri + "decimal");
            }
        }

        public static Uri Integer
        {
            get
            {
                return new Uri(BaseUri + "integer");
            }
        }

        public static Uri Long
        {
            get
            {
                return new Uri(BaseUri + "long");
            }
        }

        public static Uri Byte
        {
            get
            {
                return new Uri(BaseUri + "byte");
            }
        }

        public static Uri Short
        {
            get
            {
                return new Uri(BaseUri + "short");
            }
        }

        public static Uri DateTime
        {
            get
            {
                return new Uri(BaseUri + "dateTime");
            }
        }

        public static Uri Date
        {
            get
            {
                return new Uri(BaseUri + "date");
            }
        }

        public static Uri Boolean
        {
            get
            {
                return new Uri(BaseUri + "boolean");
            }
        }

        public static Uri Time
        {
            get
            {
                return new Uri(BaseUri + "time");
            }
        }

        public static Uri UnsignedInt
        {
            get
            {
                return new Uri(BaseUri + "unsignedInt");
            }
        }

        public static Uri UnsignedByte
        {
            get
            {
                return new Uri(BaseUri + "unsignedByte");
            }
        }

        public static Uri UnsignedLong
        {
            get
            {
                return new Uri(BaseUri + "unsignedLong");
            }
        }

        public static Uri UnsignedShort
        {
            get
            {
                return new Uri(BaseUri + "unsignedShort");
            }
        }

        public static Uri NegativeInteger
        {
            get
            {
                return new Uri(BaseUri + "unsignedInteger");
            }
        }

        public static Uri PositiveInteger
        {
            get
            {
                return new Uri(BaseUri + "positiveInteger");
            }
        }

        public static Uri NonNegativeInteger
        {
            get
            {
                return new Uri(BaseUri + "nonNegativeInteger");
            }
        }

        public static Uri NonPositiveInteger
        {
            get
            {
                return new Uri(BaseUri + "nonPositiveInteger");
            }
        }

        public static Uri Duration
        {
            get
            {
                return new Uri(BaseUri + "duration");
            }
        }
    }
}