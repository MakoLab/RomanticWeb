using System;
using System.Diagnostics.CodeAnalysis;

namespace RomanticWeb.Vocabularies
{
    /// <summary>The Web Ontology Language vocabulary (http://www.w3.org/2002/07/owl#).</summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK here.")]
    public static class Owl
    {
        // ReSharper disable InconsistentNaming
#pragma warning disable 1591
        public const string BaseUri="http://www.w3.org/2002/07/owl#";

        public static readonly Uri Thing=new Uri(BaseUri+"Thing");

        public static readonly Uri Class=new Uri(BaseUri+"Class");

        public static readonly Uri Ontology=new Uri(BaseUri+"Ontology");

        public static readonly Uri Restriction=new Uri(BaseUri+"Restriction");

        public static readonly Uri NamedIndividual=new Uri(BaseUri+"NamedIndividual");

         public static readonly Uri disjointWith=new Uri(BaseUri+"disjointWith");

        public static readonly Uri onProperty=new Uri(BaseUri+"onProperty");

        public static readonly Uri hasValue=new Uri(BaseUri+"hasValue");

        public static readonly Uri sameAs=new Uri(BaseUri+"sameAs");
#pragma warning restore 1591
        // ReSharper restore InconsistentNaming
    }
}