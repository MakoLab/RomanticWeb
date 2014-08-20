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
            registry.Register(factory => CreateMappingContext(factory), new PerScopeLifetime());
            registry.Register<IMappingsRepository, MappingsRepository>(new PerScopeLifetime());
            registry.Register<MappingModelBuilder>();

            registry.Register<IMappingModelVisitor, RdfTypeCache>("RdfTypeCache", new PerScopeLifetime());

            registry.Register<IMappingProviderVisitor, ConventionsVisitor>("ConventionsVisitor");
            registry.Register<IMappingProviderVisitor, MappingProvidersValidator>("MappingProvidersValidator");
            registry.Register<IMappingProviderVisitor, GeneratedListMappingSource>("GeneratedListMappingSource");
            registry.Register<IMappingProviderVisitor, GeneratedDictionaryMappingSource>("GeneratedDictionaryMappingSource");
        }

        private MappingContext CreateMappingContext(IServiceFactory factory)
        {
            var actualOntologyProvider = new CompoundOntologyProvider(factory.GetAllInstances<IOntologyProvider>());
            return new MappingContext(actualOntologyProvider, factory.GetAllInstances<IConvention>());
        }
    }
}