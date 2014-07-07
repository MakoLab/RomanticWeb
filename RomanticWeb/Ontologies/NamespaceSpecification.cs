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
        public NamespaceSpecification(string prefix, Uri baseUri)
        {
            Prefix = prefix;
            BaseUri = baseUri;
        }

        /// <summary>Gets the namespace URI.</summary>
        public Uri BaseUri { get; private set; }

        /// <summary>Gets the namespace prefix.</summary>
        public string Prefix { get; private set; }

#pragma warning disable 1591
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }

            return obj is NamespaceSpecification && Equals((NamespaceSpecification)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (BaseUri.GetHashCode() * 397) ^ Prefix.GetHashCode();
            }
        }
#pragma warning restore

        private bool Equals(NamespaceSpecification other)
        {
            return BaseUri.Equals(other.BaseUri) && string.Equals(Prefix, other.Prefix);
        }
    }
}