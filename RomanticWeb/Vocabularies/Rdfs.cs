using System;

namespace RomanticWeb.Vocabularies
{
    internal static class Rdfs
    {
        public const string BaseUri = "http://www.w3.org/2000/01/rdf-schema#";

        public static Uri Class
        {
            get
            {
                return new Uri(BaseUri + "Class");
            }
        }
    }
}