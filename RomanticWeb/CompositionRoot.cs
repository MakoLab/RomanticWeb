using System;
using RomanticWeb.Entities;
using RomanticWeb.LightInject;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Conventions;
using RomanticWeb.Mapping.Visitors;
using RomanticWeb.NamedGraphs;
using RomanticWeb.Ontologies;

namespace RomanticWeb
{
    public class CompositionRoot : ICompositionRoot
    {
        void ICompositionRoot.Compose(IServiceRegistry registry)
        {
            registry.Register(factory => CreateEntityContext(factory));
            registry.Register<IMappingsRepository, MappingsRepository>(new PerContainerLifetime());
            registry.Register<IMappingModelVisitor, RdfTypeCache>("RdfTypeCache", new PerContainerLifetime());
            registry.Register<IRdfTypeCache, RdfTypeCache>("RdfTypeCache", new PerContainerLifetime());
            registry.Register<IBlankNodeIdGenerator, DefaultBlankNodeIdGenerator>();
        }

        private IEntityContext CreateEntityContext(IServiceFactory factory)
        {
            var actualOntologyProvider = new CompoundOntologyProvider(factory.GetAllInstances<IOntologyProvider>());
            var mappingContext = new MappingContext(actualOntologyProvider, factory.GetAllInstances<IConvention>());
            var repository = factory.GetInstance<IMappingsRepository>();
            repository.RebuildMappings(mappingContext);

            var entitySource = factory.GetInstance<IEntitySource>();
            entitySource.MetaGraphUri = factory.GetInstance<Uri>("MetaGraphUri");

            return new EntityContext(
                factory.GetInstance<IEntityContextFactory>(),
                repository,
                mappingContext,
                factory.GetInstance<IEntityStore>(),
                entitySource,
                factory.TryGetInstance<IBaseUriSelectionPolicy>(),
                factory.GetInstance<INamedGraphSelector>(),
                (IRdfTypeCache)factory.GetInstance<IMappingModelVisitor>("RdfTypeCache"),
                factory.GetInstance<IBlankNodeIdGenerator>());
        }
    }
}