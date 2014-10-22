using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Conventions;
using RomanticWeb.Mapping.Sources;
using RomanticWeb.Mapping.Validation;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.ComponentModel
{
    internal sealed class MappingCompositionRoot : CompositionRootBase
    {
        public MappingCompositionRoot()
        {
            RdfTypeCache<RdfTypeCache>();
            MappingModelVisitor<RdfTypeCacheBuilder>();
            MappingProviderVisitor<ConventionsVisitor>();
            MappingProviderVisitor<MappingProvidersValidator>();
            MappingProviderVisitor<ConvertersRegistrator>();
            MappingProviderVisitor<GeneratedListMappingSource>();
            MappingProviderVisitor<GeneratedDictionaryMappingSource>();
        }
    }
}