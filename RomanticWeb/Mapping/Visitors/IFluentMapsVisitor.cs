using RomanticWeb.Mapping.Fluent;
using RomanticWeb.Mapping.Providers;

namespace RomanticWeb.Mapping.Visitors
{
    public interface IFluentMapsVisitor
    {
        IEntityMappingProvider Visit(EntityMap entityMap);

        IClassMappingProvider Visit(ClassMap classMap);

        IPropertyMappingProvider Visit(PropertyMap entityMap);
        
        IPropertyMappingProvider Visit(DictionaryMap dictionaryMap,ITermMappingProvider key,ITermMappingProvider value);

        IPropertyMappingProvider Visit(CollectionMap collectionMap);

        ITermMappingProvider Visit(DictionaryMap.KeyMap keyMap);

        ITermMappingProvider Visit(DictionaryMap.ValueMap valueMap);
    }
}