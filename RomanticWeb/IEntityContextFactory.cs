using System.Collections.Generic;
using RomanticWeb.Converters;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Conventions;
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
        /// Gets the conventions.
        /// </summary>
        IEnumerable<IConvention> Conventions { get; }

        /// <summary>
        /// Gets the fallback node converter.
        /// </summary>
        IFallbackNodeConverter FallbackNodeConverter { get; }

        /// <summary>Creates a new instance of entity context.</summary>
        IEntityContext CreateContext();
    }
}