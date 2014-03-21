using System;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Dynamic
{
    public class DynamicAssemblyProvider:IDictionaryPairTypeProvider
    {
        public Type GetEntryType(IPropertyMapping property)
        {
            return Type.GetType(new TypeDictionaryEntityNames(property.EntityMapping.EntityType.GetProperty(property.Name)).EntryTypeFullyQualifiedName, true);
        }

        public Type GetOwnerType(IPropertyMapping property)
        {
            return Type.GetType(new TypeDictionaryEntityNames(property.EntityMapping.EntityType.GetProperty(property.Name)).OwnerTypeFullyQualifiedName, true);
        }
    }
}