using System.Collections.Generic;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Mapping.Conventions
{
    /// <summary>
    /// Convention to ensure <see cref="IDictionary{TKey,TValue}"/> properties
    /// have the value predicate set
    /// </summary>
    public class DefaultDictionaryValuePredicateConvention : IDictionaryConvention
    {
        /// <inheritdoc/>
        /// <returns>true if <see cref="IDictionaryMappingProvider.Value"/> doesn't map to a URI</returns>
        public bool ShouldApply(IDictionaryMappingProvider target)
        {
            return target.Value.GetTerm == null;
        }

        /// <summary>
        /// Sets the key predicate to <see cref="Rdf.@object"/>
        /// </summary>
        public void Apply(IDictionaryMappingProvider target)
        {
            target.Value.GetTerm = provider => Rdf.@object;
        }
    }
}