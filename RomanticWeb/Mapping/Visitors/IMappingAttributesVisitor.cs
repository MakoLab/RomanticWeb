using System.Reflection;
using RomanticWeb.Mapping.Attributes;
using RomanticWeb.Mapping.Providers;

namespace RomanticWeb.Mapping.Visitors
{
    /// <summary>
    /// Visits mapping attributes and produces mapping providers
    /// </summary>
    public interface IMappingAttributesVisitor
    {
        /// <summary>
        /// Visits the specified attribute.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        IClassMappingProvider Visit(ClassAttribute attribute);

        /// <summary>
        /// Visits the specified property attribute.
        /// </summary>
        /// <param name="propertyAttribute">The property attribute.</param>
        /// <param name="property">The property.</param>
        IPropertyMappingProvider Visit(PropertyAttribute propertyAttribute,PropertyInfo property);

        /// <summary>
        /// Visits the specified collection attribute.
        /// </summary>
        /// <param name="collectionAttribute">The collection attribute.</param>
        /// <param name="property">The property.</param>
        ICollectionMappingProvider Visit(CollectionAttribute collectionAttribute,PropertyInfo property);

        /// <summary>
        /// Visits the specified dictionary attribute.
        /// </summary>
        /// <param name="dictionaryAttribute">The dictionary attribute.</param>
        /// <param name="property">The property.</param>
        /// <param name="key">The key property mapping provider.</param>
        /// <param name="value">The value property mapping provider.</param>
        IDictionaryMappingProvider Visit(DictionaryAttribute dictionaryAttribute,PropertyInfo property,ITermMappingProvider key,ITermMappingProvider value);

        /// <summary>
        /// Visits the specified key attribute.
        /// </summary>
        /// <param name="keyAttribute">The key attribute.</param>
        ITermMappingProvider Visit(KeyAttribute keyAttribute);

        /// <summary>
        /// Visits the specified value attribute.
        /// </summary>
        /// <param name="valueAttribute">The value attribute.</param>
        ITermMappingProvider Visit(ValueAttribute valueAttribute);
    }
}