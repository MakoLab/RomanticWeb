using System;

namespace RomanticWeb
{
    public class UnknownNamespaceException : Exception
    {
        internal UnknownNamespaceException(string namespacePrefix)
            : base(string.Format("No ontology was found for namespace prefix '{0}'", namespacePrefix))
        {
            
        }
    }
}