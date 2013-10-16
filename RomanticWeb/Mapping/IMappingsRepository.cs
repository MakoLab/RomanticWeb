using RomanticWeb.Mapping.Model;
using RomanticWeb.Ontologies;

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

        void RebuildMappings(IOntologyProvider ontologyProvider);
    }
}