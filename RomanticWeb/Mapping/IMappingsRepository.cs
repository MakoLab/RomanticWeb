using RomanticWeb.Mapping.Model;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Mapping
{
    /// <summary>
    /// Provides access to entity mappings
    /// </summary>
    public interface IMappingsRepository
    {
        /// <summary>Gets or sets an ontology provider to be used by this mappings repository to resolve namespace prefixes.</summary>
        IOntologyProvider OntologyProvider { get; set; }

        /// <summary>Gets a mapping for an Entity type.</summary>
        /// <typeparam name="TEntity">Entity type, for which mappings is going to be retrieved.</typeparam>
        IEntityMapping MappingFor<TEntity>();
    }
}