using System;
using System.Collections.Generic;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Mapping.Conventions
{
    /// <summary>Convention to ensure <see cref="IDictionary{TKey,TValue}"/> properties have the key predicate set.</summary>
    public class DefaultDictionaryKeyPredicateConvention : IDictionaryConvention
    {
        private static readonly IEnumerable<Type> Predecesors = new Type[0];

        /// <inheritdoc/>
        public IEnumerable<Type> Requires { get { return Predecesors; } }

        /// <inheritdoc/>
        /// <returns>true if <see cref="IDictionaryMappingProvider.Key"/> doesn't map to a URI</returns>
        public bool ShouldApply(IDictionaryMappingProvider target)
        {
            return target.Key.GetTerm == null;
        }

        /// <summary>Sets the key predicate to <see cref="Rdf.predicate"/>.</summary>
        public void Apply(IDictionaryMappingProvider target)
        {
            target.Key.GetTerm = provider => Rdf.predicate;
        }
    }
}