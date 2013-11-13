using System;

namespace RomanticWeb.Vocabularies
{
    internal static class Foaf
    {
        public const string BaseUri = "http://xmlns.com/foaf/0.1/";

        public static Uri Person
        {
            get
            {
                return new Uri(BaseUri+"Person");
            }
        }
    }
}