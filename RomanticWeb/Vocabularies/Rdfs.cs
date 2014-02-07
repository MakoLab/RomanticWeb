using System;
using System.Diagnostics.CodeAnalysis;

namespace RomanticWeb.Vocabularies
{
    /// <summary>The RDF Schema vocabulary (http://www.w3.org/2000/01/rdf-schema#).</summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK here.")]
    public static class Rdfs
    {
        // ReSharper disable InconsistentNaming
#pragma warning disable 1591
        public const string BaseUri="http://www.w3.org/2000/01/rdf-schema#";

        public static readonly Uri Resource=new Uri(BaseUri+"Resource");

        public static readonly Uri Class=new Uri(BaseUri+"Class");

        public static readonly Uri Literal=new Uri(BaseUri+"Literal");

        public static readonly Uri subClassOf=new Uri(BaseUri+"subClassOf");

        public static readonly Uri range=new Uri(BaseUri+"range");

        public static readonly Uri isDefinedBy=new Uri(BaseUri+"isDefinedBy");
#pragma warning restore 1591
        // ReSharper restore InconsistentNaming
    }
}