using System;
using System.Diagnostics.CodeAnalysis;

namespace RomanticWeb.Vocabularies
{
    /// <summary>The RDF Vocabulary (RDF) (http://www.w3.org/1999/02/22-rdf-syntax-ns#).</summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK here.")]
    public static partial class Rdf
    {
#pragma warning disable 1591 // ReSharper disable InconsistentNaming
        public const string BaseUri = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";

        public static readonly Uri Property = new Uri(BaseUri + "Property");

        public static readonly Uri Statement = new Uri(BaseUri + "Statement");

        public static readonly Uri Bag = new Uri(BaseUri + "Bag");

        public static readonly Uri Seq = new Uri(BaseUri + "Seq");

        public static readonly Uri Alt = new Uri(BaseUri + "Alt");

        public static readonly Uri List = new Uri(BaseUri + "List");

        public static readonly Uri type = new Uri(BaseUri + "type");

        public static readonly Uri subject = new Uri(BaseUri + "subject");

        public static readonly Uri predicate = new Uri(BaseUri + "predicate");

        public static readonly Uri @object = new Uri(BaseUri + "object");

        public static readonly Uri value = new Uri(BaseUri + "value");

        public static readonly Uri first = new Uri(BaseUri + "first");

        public static readonly Uri rest = new Uri(BaseUri + "rest");

        public static readonly Uri nil = new Uri(BaseUri + "nil");
#pragma warning restore 1591 // ReSharper restore InconsistentNaming
    }
}