using RomanticWeb.Mapping.Providers;

namespace RomanticWeb.Mapping.Visitors
{
    /// <summary>
    /// Contract for implementing visitors of mapping providers
    /// </summary>
    public interface IMappingProviderVisitor
    {
        /// <summary>
        /// Visits the specified collection mapping provider.
        /// </summary>
        /// <param name="collectionMappingProvider">The collection mapping provider.</param>
        void Visit(ICollectionMappingProvider collectionMappingProvider);

        /// <summary>
        /// Visits the specified property mapping provider.
        /// </summary>
        /// <param name="propertyMappingProvider">The property mapping provider.</param>
        void Visit(IPropertyMappingProvider propertyMappingProvider);

        /// <summary>
        /// Visits the specified dictionary mapping provider.
        /// </summary>
        /// <param name="dictionaryMappingProvider">The dictionary mapping provider.</param>
        void Visit(IDictionaryMappingProvider dictionaryMappingProvider);

        /// <summary>
        /// Visits the specified class mapping provider.
        /// </summary>
        /// <param name="classMappingProvider">The class mapping provider.</param>
        void Visit(IClassMappingProvider classMappingProvider);

        /// <summary>
        /// Visits the specified entity mapping provider.
        /// </summary>
        /// <param name="entityMappingProvider">The entity mapping provider.</param>
        void Visit(IEntityMappingProvider entityMappingProvider);
    }
}