using System;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Dynamic
{
    /// <summary>
    /// Default implementation of <see cref="IDictionaryTypeProvider"/>, 
    /// which assumes dictionary types are named according to a pattern 
    /// based on original entity type name and property details
    /// </summary>
    public class DefaultDictionaryTypeProvider:IDictionaryTypeProvider
    {
        /// <inheritdoc/>
        public Type GetEntryType(IPropertyMapping property)
        {
            return Type.GetType(new TypeDictionaryEntityNames(property.EntityMapping.EntityType.GetProperty(property.Name)).EntryTypeFullyQualifiedName, true);
        }

        /// <inheritdoc/>
        public Type GetOwnerType(IPropertyMapping property)
        {
            return Type.GetType(new TypeDictionaryEntityNames(property.EntityMapping.EntityType.GetProperty(property.Name)).OwnerTypeFullyQualifiedName, true);
        }
    }
}