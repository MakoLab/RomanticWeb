using RomanticWeb.LightInject;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Conventions;
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

            registry.Register<IMappingModelVisitor, RdfTypeCache>("RdfTypeCache", new PerScopeLifetime());

            registry.Register<IMappingProviderVisitor, ConventionsVisitor>();
            registry.Register<IMappingProviderVisitor, MappingProvidersValidator>();
            registry.Register<IMappingProviderVisitor, GeneratedListMappingSource>();
            registry.Register<IMappingProviderVisitor, GeneratedDictionaryMappingSource>();
        }

        private MappingContext CreateMappingContext(IServiceFactory factory)
        {
            var actualOntologyProvider = new CompoundOntologyProvider(factory.GetAllInstances<IOntologyProvider>());
            return new MappingContext(actualOntologyProvider, factory.GetAllInstances<IConvention>());
        }
    }
}