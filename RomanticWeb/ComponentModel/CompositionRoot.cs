using System;
using RomanticWeb.Entities;
using RomanticWeb.LightInject;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Visitors;
using RomanticWeb.NamedGraphs;

namespace RomanticWeb.ComponentModel
{
    public class CompositionRoot : ICompositionRoot
    {
        void ICompositionRoot.Compose(IServiceRegistry registry)
        {
            registry.Register(factory => CreateEntityContext(factory));
            registry.Register<IRdfTypeCache, RdfTypeCache>("RdfTypeCache", new PerScopeLifetime());
            registry.Register<IBlankNodeIdGenerator, DefaultBlankNodeIdGenerator>();
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
                    (IRdfTypeCache)factory.GetInstance<IMappingModelVisitor>("RdfTypeCache"),
                    factory.GetInstance<IBlankNodeIdGenerator>());
            }
        }
    }
}