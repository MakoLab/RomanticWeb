using System.Linq;
using RomanticWeb.Dynamic;
using RomanticWeb.LightInject;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Conventions;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Mapping.Sources;
using RomanticWeb.Mapping.Validation;
using RomanticWeb.Mapping.Visitors;
using RomanticWeb.Ontologies;

namespace RomanticWeb.ComponentModel
{
    public class MappingCompositionRoot : ICompositionRoot
    {
        void ICompositionRoot.Compose(IServiceRegistry registry)
        {
            registry.Register(factory => CreateMappingContext(factory), new PerContainerLifetime());
            registry.Register(factory => CreateMappingsRepository(factory), new PerContainerLifetime());
            registry.Register<MappingModelBuilder>();

            registry.Register<IRdfTypeCache, RdfTypeCache>(new PerContainerLifetime());
            registry.Register<IMappingModelVisitor, RdfTypeCacheBuilder>(new PerContainerLifetime());

            registry.Register(factory => CreateVisitorChain(), new PerContainerLifetime());
            registry.Register<ConventionsVisitor>();
            registry.Register<MappingProvidersValidator>();
            registry.Register<GeneratedListMappingSource>();
            registry.Register<GeneratedDictionaryMappingSource>();

            registry.Register<EmitHelper>(new PerContainerLifetime());
        }

        private static MappingContext CreateMappingContext(IServiceFactory factory)
        {
            var actualOntologyProvider = new CompoundOntologyProvider(factory.GetAllInstances<IOntologyProvider>());
            return new MappingContext(actualOntologyProvider, factory.GetAllInstances<IConvention>());
        }

        private static IMappingProviderVisitorChain CreateVisitorChain()
        {
            var chain = new MappingProviderVisitorChain();

            chain.AddFirst<ConventionsVisitor>();
            chain.AddFirst<MappingProvidersValidator>();
            chain.AddFirst<GeneratedListMappingSource>();
            chain.AddFirst<GeneratedDictionaryMappingSource>();

            return chain;
        }

        private static IMappingsRepository CreateMappingsRepository(IServiceFactory factory)
        {
            var visitors = from type in factory.GetInstance<IMappingProviderVisitorChain>().Visitors
                           select (IMappingProviderVisitor)factory.GetInstance(type);

            return new MappingsRepository(
                factory.GetInstance<MappingModelBuilder>(),
                factory.GetAllInstances<IMappingProviderSource>(),
                visitors,
                factory.GetAllInstances<IMappingModelVisitor>());
        }
    }
}