using RomanticWeb.Mapping.Fluent;
using RomanticWeb.Mapping.Providers;

namespace RomanticWeb.Mapping.Visitors
{
    /// <summary>
    /// Visits fluent entity maps and transforms them into matching entity mapping providers
    /// </summary>
    public interface IFluentMapsVisitor
    {
        /// <summary>
        /// Visits the specified entity map.
        /// </summary>
        /// <param name="entityMap">The entity map.</param>
        IEntityMappingProvider Visit(EntityMap entityMap);

        /// <summary>
        /// Visits the specified class map.
        /// </summary>
        /// <param name="classMap">The class map.</param>
        IClassMappingProvider Visit(ClassMap classMap);

        /// <summary>
        /// Visits the specified entity map.
        /// </summary>
        /// <param name="propertyMap">The entity map.</param>
        IPropertyMappingProvider Visit(PropertyMap propertyMap);

        /// <summary>
        /// Visits the specified dictionary map.
        /// </summary>
        /// <param name="dictionaryMap">The dictionary map.</param>
        /// <param name="key">The key property mapping provider.</param>
        /// <param name="value">The value property mapping provider.</param>
        IPropertyMappingProvider Visit(DictionaryMap dictionaryMap,ITermMappingProvider key,ITermMappingProvider value);

        /// <summary>
        /// Visits the specified collection map.
        /// </summary>
        /// <param name="collectionMap">The collection map.</param>
        IPropertyMappingProvider Visit(CollectionMap collectionMap);

        /// <summary>
        /// Visits the specified dictionary key property map.
        /// </summary>
        /// <param name="keyMap">The key map.</param>
        ITermMappingProvider Visit(DictionaryMap.KeyMap keyMap);

        /// <summary>
        /// Visits the specified dictionary value property map.
        /// </summary>
        /// <param name="valueMap">The value map.</param>
        /// <returns></returns>
        ITermMappingProvider Visit(DictionaryMap.ValueMap valueMap);
    }
}