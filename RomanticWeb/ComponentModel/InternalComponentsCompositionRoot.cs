using System;
using System.Linq;
using RomanticWeb.Converters;
using RomanticWeb.Dynamic;
using RomanticWeb.Entities;
using RomanticWeb.Entities.ResultAggregations;
using RomanticWeb.LightInject;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Conventions;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Mapping.Sources;
using RomanticWeb.Mapping.Visitors;
using RomanticWeb.NamedGraphs;
using RomanticWeb.Ontologies;
using RomanticWeb.Updates;

namespace RomanticWeb.ComponentModel
{
    internal class InternalComponentsCompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry registry)
        {
            registry.Register<IConverterCatalog, ConverterCatalog>(new PerContainerLifetime());

            registry.Register<IResultTransformerCatalog, ResultTransformerCatalog>(new PerContainerLifetime());
            RegisterResultAggregator<AnyResultCheck>(registry);
            RegisterResultAggregator<FirstOrDefault>(registry);
            RegisterResultAggregator<FirstResult>(registry);
            RegisterResultAggregator<SingleOrDefault>(registry);
            RegisterResultAggregator<SingleResult>(registry);

            registry.Register(factory => CreateEntitySource(factory));

            registry.Register<EmitHelper>(new PerContainerLifetime());

            registry.Register<MappingModelBuilder>();
            registry.Register(factory => CreateMappingContext(factory), new PerContainerLifetime());
            registry.Register(factory => CreateMappingsRepository(factory), new PerContainerLifetime());

            registry.Register<IEntityCaster, ImpromptuInterfaceCaster>();

            registry.Register(factory => CreateEntityProxy(factory));

            registry.Register<IDatasetChangesTracker, DatasetChanges>();
            registry.Register<IDatasetChangesOptimizer, DatasetChangesOptimizer>();
        }

        private static Func<Entity, IEntityMapping, IEntityProxy> CreateEntityProxy(IServiceFactory factory)
        {
            return (entity, mapping) =>
                {
                    var transformerCatalog = factory.GetInstance<IResultTransformerCatalog>();
                    var namedGraphSeletor = factory.GetInstance<INamedGraphSelector>();
                    return new EntityProxy(entity, mapping, transformerCatalog, namedGraphSeletor);
                };
        }

        private static IEntitySource CreateEntitySource(IServiceFactory factory)
        {
            var entitySource = factory.GetInstance<IEntitySource>("EntitySource");
            entitySource.MetaGraphUri = factory.GetInstance<Uri>("MetaGraphUri");
            return entitySource;
        }

        private static void RegisterResultAggregator<T>(IServiceRegistry registry) where T : IResultAggregator
        {
            registry.Register<IResultAggregator, T>(typeof(T).FullName, new PerContainerLifetime());
        }

        private static MappingContext CreateMappingContext(IServiceFactory factory)
        {
            var actualOntologyProvider = new CompoundOntologyProvider(factory.GetAllInstances<IOntologyProvider>());
            return new MappingContext(actualOntologyProvider, factory.GetAllInstances<IConvention>());
        }

        private static IMappingsRepository CreateMappingsRepository(IServiceFactory factory)
        {
            var visitors = from chain in factory.GetAllInstances<MappingProviderVisitorChain>()
                           from type in chain.Visitors
                           select (IMappingProviderVisitor)factory.GetInstance(type);

            return new MappingsRepository(
                factory.GetInstance<MappingModelBuilder>(),
                factory.GetAllInstances<IMappingProviderSource>(),
                visitors,
                factory.GetAllInstances<IMappingModelVisitor>());
        }
    }
}