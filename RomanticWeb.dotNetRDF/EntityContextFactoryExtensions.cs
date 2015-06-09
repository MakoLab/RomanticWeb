using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using RomanticWeb.ComponentModel;
using RomanticWeb.LinkedData;
using RomanticWeb.Mapping;
using VDS.RDF;

namespace RomanticWeb.DotNetRDF
{
    /// <summary>
    /// </summary>
    public static class EntityContextFactoryExtensions
    {
        /// <summary>
        /// Sets up the <paramref name="factory"/> with components required to use dotNetRDF
        /// </summary>
        public static EntityContextFactory WithDotNetRDF(this EntityContextFactory factory)
        {
            return factory.WithDependencies<Components>()
                          .WithEntitySource<TripleStoreAdapter>();
        }

        /// <summary>
        /// Sets up the <paramref name="factory"/> with components required to use dotNetRDF and supplies a triple store instance
        /// </summary>
        public static EntityContextFactory WithDotNetRDF(this EntityContextFactory factory, ITripleStore store)
        {
            ((IComponentRegistryFacade)factory).Register(store);
            return WithDotNetRDF(factory);
        }

        /// <summary>
        /// Sets up the <paramref name="factory"/> with components required to use dotNetRDF 
        /// and supplies a triple store name configured in app.config/web.config
        /// </summary>
        public static EntityContextFactory WithDotNetRDF(this EntityContextFactory factory, string storeName)
        {
            ((IComponentRegistryFacade)factory).Register(Configuration.StoresConfigurationSection.Default.CreateStore(storeName));
            return WithDotNetRDF(factory);
        }

        /// <summary>Sets up the factory to use <see cref="BaseUriResolutionStrategyComposition" /> for resolving external resources.</summary>
        /// <remarks>
        /// This implementation checks if a resource's identifier matches given <paramref name="baseUris" /> 
        /// and then resolves by making a <see cref="WebRequest" /> to resource's identifier.
        /// </remarks>
        /// <param name="factory">Target factory to be configured.</param>
        /// <param name="baseUris">Base Uris to match for external resources.</param>
        /// <returns>Given <paramref name="factory" />.</returns>
        public static EntityContextFactory WithUriMatchingResourceResulutionStrategy(this EntityContextFactory factory, IEnumerable<Uri> baseUris)
        {
            factory.WithDependencies<BaseUriResolutionStrategyComposition>();
            var resolutionStrategy = new UrlMatchingResourceResolutionStrategy(
                factory.Ontologies,
                factory.MappingModelVisitors.OfType<BaseUriMappingModelVisitor>().First().MappingAssemblies,
                baseUris);
            return factory.WithResourceResolutionStrategy(resolutionStrategy);
        }
    }
}