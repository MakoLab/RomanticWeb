using System;
using System.Diagnostics.CodeAnalysis;

namespace RomanticWeb.Vocabularies
{
    /// <summary>The W3C Linked Data Platform (LDP) Vocabulary (http://www.w3.org/ns/ldp#).</summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules","*",Justification="Reviewed. Suppression is OK here.")]
    public static class Ldp
    {
#pragma warning disable 1591 // ReSharper disable InconsistentNaming
        public const string BaseUri="http://www.w3.org/ns/ldp#";

        public static readonly Uri Resource=new Uri(BaseUri+"Resource");

        public static readonly Uri Container=new Uri(BaseUri+"Container");

        public static readonly Uri AggregateContainer=new Uri(BaseUri+"AggregateContainer");

        public static readonly Uri CompositeContainer=new Uri(BaseUri+"CompositeContainer");

        public static readonly Uri Page=new Uri(BaseUri+"Page");

        public static readonly Uri containerSortPredicates=new Uri(BaseUri+"containerSortPredicates");

        public static readonly Uri membershipPredicate=new Uri(BaseUri+"membershipPredicate");

        public static readonly Uri membershipSubject=new Uri(BaseUri+"membershipSubject");

        public static readonly Uri nextPage=new Uri(BaseUri+"nextPage");

        public static readonly Uri pageOf=new Uri(BaseUri+"pageOf");
#pragma warning restore 1591 // ReSharper restore InconsistentNaming
    }
}