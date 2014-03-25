using System.Collections.Generic;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Mapping.Conventions
{
    /// <summary>
    /// Convention to ensure <see cref="IDictionary{TKey,TValue}"/> properties
    /// have the key predicate set
    /// </summary>
    public class DefaultDictionaryKeyPredicateConvention:IDictionaryConvention
    {
        /// <inheritdoc/>
        /// <returns>true if <see cref="IDictionaryMappingProvider.Key"/> doesn't map to a URI</returns>
        public bool ShouldApply(IDictionaryMappingProvider target)
        {
            return target.Key.GetTerm==null;
        }

        /// <summary>
        /// Sets the key predicate to <see cref="Rdf.predicate"/>
        /// </summary>
        public void Apply(IDictionaryMappingProvider target)
        {
            target.Key.GetTerm=provider => Rdf.predicate;
        }
    }
}