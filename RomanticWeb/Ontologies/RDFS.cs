using System;
using System.Diagnostics.CodeAnalysis;

namespace RomanticWeb.Vocabularies
{
    /// <summary>The RDF Schema vocabulary (RDFS) (http://www.w3.org/2000/01/rdf-schema#).</summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules","*",Justification="Reviewed. Suppression is OK here.")]
    public static class Rdfs
    {
#pragma warning disable 1591 // ReSharper disable InconsistentNaming
        public const string BaseUri="http://www.w3.org/2000/01/rdf-schema#";

        public static readonly Uri Resource=new Uri(BaseUri+"Resource");

        public static readonly Uri Class=new Uri(BaseUri+"Class");

        public static readonly Uri Literal=new Uri(BaseUri+"Literal");

        public static readonly Uri Container=new Uri(BaseUri+"Container");

        public static readonly Uri ContainerMembershipProperty=new Uri(BaseUri+"ContainerMembershipProperty");

        public static readonly Uri Datatype=new Uri(BaseUri+"Datatype");

        public static readonly Uri subClassOf=new Uri(BaseUri+"subClassOf");

        public static readonly Uri subPropertyOf=new Uri(BaseUri+"subPropertyOf");

        public static readonly Uri comment=new Uri(BaseUri+"comment");

        public static readonly Uri label=new Uri(BaseUri+"label");

        public static readonly Uri domain=new Uri(BaseUri+"domain");

        public static readonly Uri range=new Uri(BaseUri+"range");

        public static readonly Uri seeAlso=new Uri(BaseUri+"seeAlso");

        public static readonly Uri isDefinedBy=new Uri(BaseUri+"isDefinedBy");

        public static readonly Uri member=new Uri(BaseUri+"member");
#pragma warning restore 1591 // ReSharper restore InconsistentNaming
    }
}