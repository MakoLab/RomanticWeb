using System;
using System.Diagnostics;

namespace RomanticWeb.Ontologies
{
    /// <summary>
    /// Represents a prefix-URI pair used for defining namespaces
    /// </summary>
    [DebuggerDisplay("PREFIX {Prefix}: <{BaseUri}>")]
    public sealed class NamespaceSpecification
    {
        /// <summary>
        /// Creates a new insance of <see cref="NamespaceSpecification"/>
        /// </summary>
        public NamespaceSpecification(string prefix, string baseUri)
        {
            Prefix = prefix;
            BaseUri = new Uri(baseUri);
        }

        /// <summary>
        /// Gets the namespace URI
        /// </summary>
        public Uri BaseUri { get; private set; }

        /// <summary>
        /// Gets the namespace prefix
        /// </summary>
        public string Prefix { get; private set; }
    }
}