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
            registry.Register<IMappingsRepository, MappingsRepository>(new PerContainerLifetime());
            registry.Register<MappingModelBuilder>();

            registry.Register<IRdfTypeCache, RdfTypeCache>(new PerContainerLifetime());
            registry.Register<IMappingModelVisitor, RdfTypeCacheBuilder>(new PerContainerLifetime());

            registry.Register<IMappingProviderVisitor, ConventionsVisitor>("ConventionsVisitor");
            registry.Register<IMappingProviderVisitor, MappingProvidersValidator>("MappingProvidersValidator");
            registry.Register<IMappingProviderVisitor, GeneratedListMappingSource>("GeneratedListMappingSource");
            registry.Register<IMappingProviderVisitor, GeneratedDictionaryMappingSource>("GeneratedDictionaryMappingSource");

            registry.Register<EmitHelper>(new PerContainerLifetime());
        }

        private MappingContext CreateMappingContext(IServiceFactory factory)
        {
            var actualOntologyProvider = new CompoundOntologyProvider(factory.GetAllInstances<IOntologyProvider>());
            return new MappingContext(actualOntologyProvider, factory.GetAllInstances<IConvention>());
        }
    }
}