using System;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Mapping.Fluent
{
    /// <summary>
    /// Base class for mapping definitions of rdf terms
    /// </summary>
    public abstract class TermMap
    {
        private Uri TermUri { get; set; }

        private string NamespacePrefix { get; set; }

        private string TermName { get; set; }

        internal void SetUri(Uri uri)
        {
            TermUri = uri;
        }

        internal void SetQName(string prefix, string termName)
        {
            NamespacePrefix=prefix;
            TermName=termName;
        }

        /// <summary>
        /// Gets the <see cref="TermUri"/>, if set explicitly 
        /// or uses the <paramref name="ontologyProvider"/> to resolve the QName
        /// </summary>
        protected Uri GetTermUri(IOntologyProvider ontologyProvider)
        {
            return TermUri ?? ontologyProvider.ResolveUri(NamespacePrefix, TermName);
        }
    }
}