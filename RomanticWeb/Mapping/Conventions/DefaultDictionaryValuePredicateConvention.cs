using System;
using System.Collections.Generic;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Mapping.Conventions
{
    /// <summary>Convention to ensure <see cref="IDictionary{TKey,TValue}"/> properties have the value predicate set.</summary>
    public class DefaultDictionaryValuePredicateConvention : IDictionaryConvention
    {
        private static readonly IEnumerable<Type> Predecesors = new Type[0];

        /// <inheritdoc/>
        public IEnumerable<Type> Requires { get { return Predecesors; } }

        /// <inheritdoc/>
        /// <returns>true if <see cref="IDictionaryMappingProvider.Value"/> doesn't map to a URI</returns>
        public bool ShouldApply(IDictionaryMappingProvider target)
        {
            return target.Value.GetTerm == null;
        }

        /// <summary>Sets the key predicate to <see cref="Rdf.@object"/>.</summary>
        public void Apply(IDictionaryMappingProvider target)
        {
            target.Value.GetTerm = provider => Rdf.@object;
        }
    }
}