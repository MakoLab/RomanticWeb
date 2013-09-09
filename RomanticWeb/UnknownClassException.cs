using System;

namespace RomanticWeb
{
    /// <summary>
    /// The exception, which is thrown when an RDF class is accessed and cannot be found
    /// </summary>
    public class UnknownClassException : Exception
    {
        internal UnknownClassException(Uri ontologyUri, string className)
            : base(string.Format("Unknown rdf class '{0}'", new Uri(ontologyUri + className)))
        {
        }
    }
}