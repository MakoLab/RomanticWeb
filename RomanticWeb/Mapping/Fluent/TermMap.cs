using System;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Mapping.Fluent
{
    /// <summary>
    /// Base class for mapping definitions of rdf terms
    /// </summary>
    public abstract class TermMap
    {
        internal Uri TermUri { get; set; }

        internal string NamespacePrefix { get; set; }

        internal string TermName { get; set; }

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
            if (TermUri!=null)
            {
                return TermUri;
            }

            Uri resolvedUri=ontologyProvider.ResolveUri(NamespacePrefix,TermName);

            if (resolvedUri==null)
            {
                throw new MappingException(string.Format("Cannot resolved QName {0}:{1}",NamespacePrefix,TermName));
            }

            return resolvedUri;
        }
    }
}