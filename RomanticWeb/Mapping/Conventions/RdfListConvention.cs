using System.Collections.Generic;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Mapping.Providers;

namespace RomanticWeb.Mapping.Conventions
{
    /// <summary>
    /// Convention to ensure <see cref="ICollection{T}"/> and <see cref="IEnumerable{T}"/> properties
    /// are stored and read as rdf:List objects
    /// </summary>
    public class RdfListConvention : ICollectionConvention
    {
        /// <inheritdoc/>
        public bool ShouldApply(ICollectionMappingProvider target)
        {
            return target.StoreAs == StoreAs.Undefined
                && target.PropertyInfo.PropertyType.IsGenericType
                && target.PropertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(IList<>);
        }

        /// <inheritdoc/>
        public void Apply(ICollectionMappingProvider target)
        {
            target.StoreAs = StoreAs.RdfList;
        }
    }
}