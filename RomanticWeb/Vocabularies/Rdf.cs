using System;
using System.Diagnostics.CodeAnalysis;

namespace RomanticWeb.Vocabularies
{
    /// <summary>The Resource Description Framework vocabulary (http://www.w3.org/1999/02/22-rdf-syntax-ns#).</summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK here.")]
    public static class Rdf
    {
        // ReSharper disable InconsistentNaming
#pragma warning disable 1591
        public const string BaseUri="http://www.w3.org/1999/02/22-rdf-syntax-ns#";

        public static readonly Uri first=new Uri(BaseUri+"first");

        public static readonly Uri nil=new Uri(BaseUri+"nil");

        public static readonly Uri rest=new Uri(BaseUri+"rest");

        public static readonly Uri type=new Uri(BaseUri+"type");

        public static readonly Uri about=new Uri(BaseUri+"about");

        public static readonly Uri subject=new Uri(BaseUri+"subject");

        public static readonly Uri predicate=new Uri(BaseUri+"predicate");

        public static readonly Uri @object=new Uri(BaseUri+"object");
#pragma warning restore 1591
        // ReSharper restore InconsistentNaming
    }
}