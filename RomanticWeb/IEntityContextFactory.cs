using System;
using System.Collections.Generic;
using RomanticWeb.Converters;
using RomanticWeb.Entities;
using RomanticWeb.LinkedData;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Conventions;
using RomanticWeb.Mapping.Visitors;
using RomanticWeb.NamedGraphs;
using RomanticWeb.Ontologies;

namespace RomanticWeb
{
    /// <summary>An entrypoint to RomanticWeb, which encapsulates modularity and creation of <see cref="IEntityContext" />.</summary>
    public interface IEntityContextFactory : IDisposable
    {
        /// <summary>Gets the ontology provider.</summary>
        IOntologyProvider Ontologies { get; }

        /// <summary>Gets the mappings.</summary>
        IMappingsRepository Mappings { get; }

        /// <summary>Gets the conventions.</summary>
        IEnumerable<IConvention> Conventions { get; }

        /// <summary>Gets the fallback node converter.</summary>
        IFallbackNodeConverter FallbackNodeConverter { get; }

        /// <summary>Gets the mapping model visitors.</summary>
        IEnumerable<IMappingModelVisitor> MappingModelVisitors { get; }

        /// <summary>Gets the transformer catalog.</summary>
        IResultTransformerCatalog TransformerCatalog { get; }

        /// <summary>Gets the transformer catalog.</summary>
        INamedGraphSelector NamedGraphSelector { get; }

        /// <summary>Gets the external resource resolution strategy.</summary>
        IResourceResolutionStrategy ResourceResolutionStrategy { get; }

        /// <summary>Creates a new instance of entity context.</summary>
        IEntityContext CreateContext();
    }
}