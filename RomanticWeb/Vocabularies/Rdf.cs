using System;

namespace RomanticWeb.Vocabularies
{
    /// <summary>
    /// The RDF vocabulary (http://www.w3.org/1999/02/22-rdf-syntax-ns#)
    /// </summary>
    public static class Rdf
    {
#pragma warning disable 1591
        public const string BaseUri="http://www.w3.org/1999/02/22-rdf-syntax-ns#";

        public static readonly Uri First=new Uri(BaseUri+"first");

        public static readonly Uri Nil=new Uri(BaseUri+"nil");

        public static readonly Uri Rest=new Uri(BaseUri+"rest");

        public static readonly Uri Type=new Uri(BaseUri+"type");

        public static readonly Uri About = new Uri(BaseUri + "about");
#pragma warning restore 1591
    }
}