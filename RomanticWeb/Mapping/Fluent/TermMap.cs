using System;

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
    }
}