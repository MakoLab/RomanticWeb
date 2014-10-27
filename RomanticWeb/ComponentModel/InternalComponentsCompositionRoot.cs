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
    internal delegate void RegisterConverterDelegate(Type converterType);

    internal class InternalComponentsCompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry registry)
        {
            registry.RegisterInstance<RegisterConverterDelegate>(type => RegisterConverter(type, registry));
            registry.Register<IConverterCatalog, ConverterCatalog>(new PerContainerLifetime());

            registry.Register<IResultTransformerCatalog, ResultTransformerCatalog>(new PerContainerLifetime());
            registry.RegisterAssembly(GetType().Assembly, () => new PerContainerLifetime(), (service, impl) => service == typeof(IResultAggregator));

            registry.Register(factory => CreateEntitySource(factory), new PerContainerLifetime());

            registry.Register<EmitHelper>(new PerContainerLifetime());

            registry.Register<MappingModelBuilder>();
            registry.Register(factory => CreateMappingContext(factory), new PerContainerLifetime());
            registry.Register(factory => CreateMappingsRepository(factory), new PerContainerLifetime());

            registry.Register<IEntityCaster, ImpromptuInterfaceCaster>(new PerScopeLifetime());

            registry.Register(factory => CreateEntityProxy(factory));

            registry.Register<IDatasetChangesTracker, DatasetChanges>(new PerScopeLifetime());
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

        private void RegisterConverter(Type converterType, IServiceRegistry registry)
        {
            if (converterType == null || IsAlreadyRegistered(converterType, registry))
            {
                return;
            }

            registry.Register(converterType, converterType, converterType.FullName, new PerContainerLifetime());
        }

        private bool IsAlreadyRegistered(Type converterType, IServiceRegistry registry)
        {
            return registry.AvailableServices.Any(s => s.ServiceName == converterType.FullName);
        }
    }
}