using RomanticWeb.Converters;
using RomanticWeb.Entities;
using RomanticWeb.Mapping;
using RomanticWeb.Ontologies;

namespace RomanticWeb
{
    /// <summary>An entrypoint to RomanticWeb, which encapsulates modularity and creation of <see cref="IEntityContext" />.</summary>
    public interface IEntityContextFactory
    {
        /// <summary>Gets the ontology provider.</summary>
        IOntologyProvider Ontologies { get; }

        /// <summary>Gets the mappings.</summary>
        IMappingsRepository Mappings { get; }

        /// <summary>
        /// Gets the converters.
        /// </summary>
        /// <value>
        /// The converters.
        /// </value>
        IConverterCatalog Converters { get; }

        /// <summary>
        /// Gets the transformer catalog.
        /// </summary>
        /// <value>
        /// The transformer catalog.
        /// </value>
        IResultTransformerCatalog TransformerCatalog { get; }

        /// <summary>Creates a new instance of entity context.</summary>
        IEntityContext CreateContext();
    }
}