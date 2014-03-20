using RomanticWeb.Mapping.Providers;

namespace RomanticWeb.Mapping.Visitors
{
    public interface IMappingProviderVisitor
    {
        void Visit(ICollectionMappingProvider mappingProvider);

        void Visit(IPropertyMappingProvider mappingProvider);

        void Visit(IDictionaryMappingProvider mappingProvider);

        void Visit(IClassMappingProvider mappingProvider);

        void Visit(IEntityMappingProvider mappingProvider);
    }
}