using System;
using RomanticWeb.Entities;
using RomanticWeb.LightInject;
using RomanticWeb.Mapping;
using RomanticWeb.NamedGraphs;
using RomanticWeb.Ontologies;

namespace RomanticWeb.ComponentModel
{
    public class CompositionRoot : ICompositionRoot
    {
        void ICompositionRoot.Compose(IServiceRegistry registry)
        {
            registry.Register(factory => CreateEntityContext(factory));
            registry.Register<IBlankNodeIdGenerator, DefaultBlankNodeIdGenerator>();
            registry.Register<IOntologyProvider, DefaultOntologiesProvider>("DefaultOntologiesProvider");
            registry.Register<INamedGraphSelector, NamedGraphSelector>();
            registry.Register<IEntityStore, EntityStore>();
        }

        private IEntityContext CreateEntityContext(IServiceFactory factory)
        {
            using (factory.BeginScope())
            {
                var entitySource = factory.GetInstance<IEntitySource>();
                entitySource.MetaGraphUri = factory.GetInstance<Uri>("MetaGraphUri");

                return new EntityContext(
                    factory.GetInstance<IEntityContextFactory>(),
                    factory.GetInstance<IMappingsRepository>(),
                    factory.GetInstance<MappingContext>(),
                    factory.GetInstance<IEntityStore>(),
                    entitySource,
                    factory.TryGetInstance<IBaseUriSelectionPolicy>(),
                    factory.GetInstance<INamedGraphSelector>(),
                    factory.GetInstance<IRdfTypeCache>(),
                    factory.GetInstance<IBlankNodeIdGenerator>(),
                    factory.GetInstance<IResultTransformerCatalog>());
            }
        }
    }
}