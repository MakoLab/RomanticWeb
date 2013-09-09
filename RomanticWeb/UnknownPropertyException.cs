using System;

namespace RomanticWeb
{
    /// <summary>
    /// Exceptiopn thrown when a property is accessed, which cannot be found
    /// </summary>
    public class UnknownPropertyException : Exception
    {
        internal UnknownPropertyException(Uri ontologyUri, string predicate)
            : base(string.Format("Predicate {0} was not found in the ontology", new Uri(ontologyUri + predicate)))
        {
            
        }
    }
}