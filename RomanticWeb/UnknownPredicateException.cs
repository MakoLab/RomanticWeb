using System;

namespace RomanticWeb
{
    public class UnknownPredicateException : Exception
    {
        internal UnknownPredicateException(Uri ontologyUri, string predicate)
            : base(string.Format("Predicate {0} was not found in the ontology", new Uri(ontologyUri, predicate)))
        {
            
        }
    }
}