using System;

namespace RomanticWeb
{
    /// <summary>
    /// Exceptiopn thrown when an ontology is accessed, whose namespace cannot be found
    /// </summary>
    public class UnknownNamespaceException : Exception
    {
        internal UnknownNamespaceException(string namespacePrefix)
            : base(string.Format("No ontology was found for namespace prefix '{0}'", namespacePrefix))
        {

        }
    }
}