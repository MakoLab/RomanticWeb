using System;

namespace RomanticWeb.Vocabularies
{
    /// <summary>The Friend of a Friend vocabulary (http://xmlns.com/foaf/0.1/).</summary>
    public static class Foaf
    {
#pragma warning disable 1591
        public const string BaseUri="http://xmlns.com/foaf/0.1/";

        public static Uri Person { get { return new Uri(BaseUri+"Person"); } }
#pragma warning restore 1591
    }
}