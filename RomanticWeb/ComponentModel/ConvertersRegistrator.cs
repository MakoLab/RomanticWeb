using RomanticWeb.Mapping.Providers;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.ComponentModel
{
    /// <summary>
    /// Collects all node converteres so that they can be registered with the container
    /// </summary>
    internal class ConvertersRegistrator : IMappingProviderVisitor
    {
        private readonly RegisterConverterDelegate _registerConverter;

        public ConvertersRegistrator(RegisterConverterDelegate registerConverter)
        {
            _registerConverter = registerConverter;
        }

        public void Visit(ICollectionMappingProvider collectionMappingProvider)
        {
            _registerConverter(collectionMappingProvider.ElementConverterType);
        }

        public void Visit(IPropertyMappingProvider propertyMappingProvider)
        {
            _registerConverter(propertyMappingProvider.ConverterType);
        }

        public void Visit(IDictionaryMappingProvider dictionaryMappingProvider)
        {
            _registerConverter(dictionaryMappingProvider.Key.ConverterType);
            _registerConverter(dictionaryMappingProvider.Value.ConverterType);
        }

        public void Visit(IClassMappingProvider classMappingProvider)
        {
        }

        public void Visit(IEntityMappingProvider entityMappingProvider)
        {
        }
    }
}