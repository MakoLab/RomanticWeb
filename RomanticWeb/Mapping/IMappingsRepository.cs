using System;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Mapping
{
    /// <summary>
    /// Provides access to entity mappings
    /// </summary>
    public interface IMappingsRepository
    {
        /// <summary>Gets a mapping for an Entity type.</summary>
        /// <typeparam name="TEntity">Entity type, for which mappings is going to be retrieved.</typeparam>
        IEntityMapping MappingFor<TEntity>();

        /// <summary>
        /// Builds mappings and resolves all URIs against a given ontology provider
        /// </summary>
        void RebuildMappings(MappingContext mappingContext);

        /// <summary>Gets a mapping for an Entity type.</summary>
        IEntityMapping MappingFor(Type entityType);
    }
}