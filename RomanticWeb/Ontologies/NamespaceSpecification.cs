using System;
using System.Diagnostics;

namespace RomanticWeb.Ontologies
{
    /// <summary>
    /// Represents a prefix-URI pair used for defining namespaces
    /// </summary>
    [DebuggerDisplay("@prefix {Prefix,nq}: <{BaseUri,nq}>")]
    public sealed class NamespaceSpecification
    {
        /// <summary>Creates a new insance of <see cref="NamespaceSpecification"/>.</summary>
        public NamespaceSpecification(string prefix, string baseUri)
        {
            Prefix = prefix;
            BaseUri = new Uri(baseUri);
        }

        /// <summary>Gets the namespace URI.</summary>
        public Uri BaseUri { get; private set; }

        /// <summary>Gets the namespace prefix.</summary>
        public string Prefix { get; private set; }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Prefix.GetHashCode()^BaseUri.AbsoluteUri.GetHashCode();
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj,null)) { return true; }
            if (ReferenceEquals(obj,this)) { return true; }
            if (!(obj is NamespaceSpecification)) { return false; }
            NamespaceSpecification namespaceSpecification=(NamespaceSpecification)obj;
            return Prefix.Equals(namespaceSpecification.Prefix)&&(BaseUri.AbsoluteUri.Equals(namespaceSpecification.BaseUri.AbsoluteUri));
        }
    }
}