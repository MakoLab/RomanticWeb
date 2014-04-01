using System;
using System.Diagnostics.CodeAnalysis;

namespace RomanticWeb.Vocabularies
{
    /// <summary>Representing Content in RDF (http://www.w3.org/2011/content#).</summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules","*",Justification="Reviewed. Suppression is OK here.")]
    public static partial class Cnt
    {
#pragma warning disable 1591 // ReSharper disable InconsistentNaming
        public const string BaseUri="http://www.w3.org/2011/content#";

        public static readonly Uri ContentAsBase64=new Uri(BaseUri+"ContentAsBase64");

        public static readonly Uri ContentAsText=new Uri(BaseUri+"ContentAsText");

        public static readonly Uri ContentAsXML=new Uri(BaseUri+"ContentAsXML");

        public static readonly Uri DoctypeDecl=new Uri(BaseUri+"DoctypeDecl");

        public static readonly Uri bytes=new Uri(BaseUri+"bytes");

        public static readonly Uri characterEncoding=new Uri(BaseUri+"characterEncoding");

        public static readonly Uri chars=new Uri(BaseUri+"chars");

        public static readonly Uri declaredEncoding=new Uri(BaseUri+"declaredEncoding");

        public static readonly Uri doctypeName=new Uri(BaseUri+"doctypeName");

        public static readonly Uri dtDecl=new Uri(BaseUri+"dtDecl");

        public static readonly Uri internalSubset=new Uri(BaseUri+"internalSubset");

        public static readonly Uri leadingMisc=new Uri(BaseUri+"leadingMisc");

        public static readonly Uri publicId=new Uri(BaseUri+"publicId");

        public static readonly Uri rest=new Uri(BaseUri+"rest");

        public static readonly Uri standalone=new Uri(BaseUri+"standalone");

        public static readonly Uri systemId=new Uri(BaseUri+"systemId");

        public static readonly Uri version=new Uri(BaseUri+"version");
#pragma warning restore 1591 // ReSharper restore InconsistentNaming
    }
}