using System;

namespace RomanticWeb.Vocabularies
{
    internal static class Rdf
    {
        public const string BaseUri = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";

        public static Uri First
        {
            get
            {
                return new Uri(BaseUri + "first");
            }
        }

        public static Uri Nil
        {
            get
            {
                return new Uri(BaseUri + "nil");
            }
        }

        public static Uri Rest
        {
            get
            {
                return new Uri(BaseUri+"rest");
            }
        }
    }
}