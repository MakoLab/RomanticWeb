using System.Reflection;
using RomanticWeb.Mapping.Attributes;
using RomanticWeb.Mapping.Providers;

namespace RomanticWeb.Mapping.Visitors
{
    public interface IMappingAttributesVisitor
    {
        IClassMappingProvider Visit(ClassAttribute attribute);

        IPropertyMappingProvider Visit(PropertyInfo property,PropertyAttribute attribute);

        ICollectionMappingProvider Visit(PropertyInfo property,CollectionAttribute attribute);

        IDictionaryMappingProvider Visit(PropertyInfo property,DictionaryAttribute attribute,ITermMappingProvider key,ITermMappingProvider value);

        ITermMappingProvider Visit(KeyAttribute attribute);

        ITermMappingProvider Visit(ValueAttribute attribute);
    }
}