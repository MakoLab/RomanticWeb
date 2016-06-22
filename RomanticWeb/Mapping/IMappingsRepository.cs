using System;
using System.Collections.Generic;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Mapping
{
    /// <summary>Provides access to entity mappings.</summary>
    public interface IMappingsRepository : IEnumerable<IEntityMapping>
    {
        /// <summary>Gets a mapping for an Entity type.</summary>
        /// <typeparam name="TEntity">Entity type, for which mappings is going to be retrieved.</typeparam>
        IEntityMapping MappingFor<TEntity>();

        /// <summary>Gets a mapping for an Entity type.</summary>
        IEntityMapping MappingFor(Type entityType);

        /// <summary>Gets a property mapping for given predicate Uri.</summary>
        /// <param name="predicateUri">Predicate Uri to be search for.</param>
        /// <returns>Property mapped to given predicate Uri or null.</returns>
        IPropertyMapping MappingForProperty(Uri predicateUri);
    }
}