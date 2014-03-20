using System.Collections.Generic;
using System.Linq;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Fluent
{
    internal class FluentMappingProviderBuilder:IFluentMapsVisitor
    {
        public IEntityMappingProvider Visit(EntityMap entityMap)
        {
            return new EntityMappingProvider(entityMap.Type, GetClasses(entityMap), GetProperties(entityMap));
        }

        public IClassMappingProvider Visit(ClassMap classMap)
        {
            if (classMap.TermUri != null)
            {
                return new ClassMappingProvider(classMap.TermUri);
            }

            return new ClassMappingProvider(classMap.NamespacePrefix,classMap.TermName);
        }

        public IPropertyMappingProvider Visit(PropertyMap entityMap)
        {
            if (entityMap.TermUri != null)
            {
                return new PropertyMappingProvider(entityMap.TermUri, entityMap.PropertyInfo);
            }

            return new PropertyMappingProvider(entityMap.NamespacePrefix, entityMap.TermName, entityMap.PropertyInfo);
        }

        public IPropertyMappingProvider Visit(DictionaryMap dictionaryMap,ITermMappingProvider key,ITermMappingProvider value)
        {
            if (dictionaryMap.TermUri != null)
            {
                return new DictionaryMappingProvider(dictionaryMap.TermUri, key, value, dictionaryMap.PropertyInfo);
            }

            return new DictionaryMappingProvider(dictionaryMap.NamespacePrefix, dictionaryMap.TermName, key, value, dictionaryMap.PropertyInfo);
        }

        public IPropertyMappingProvider Visit(CollectionMap collectionMap)
        {
            if (collectionMap.TermUri != null)
            {
                return new CollectionMappingProvider(collectionMap.TermUri, collectionMap.StorageStrategy, collectionMap.PropertyInfo);
            }

            return new CollectionMappingProvider(
                collectionMap.NamespacePrefix,
                collectionMap.TermName,
                collectionMap.StorageStrategy,
                collectionMap.PropertyInfo);
        }

        public ITermMappingProvider Visit(DictionaryMap.KeyMap keyMap)
        {
            if (keyMap.TermUri!=null)
            {
                return new KeyMappingProvider(keyMap.TermUri);
            }
            
            if (keyMap.NamespacePrefix != null && keyMap.TermName != null)
            {
                return new KeyMappingProvider(keyMap.NamespacePrefix,keyMap.TermName);
            }

            return new KeyMappingProvider();
        }

        public ITermMappingProvider Visit(DictionaryMap.ValueMap valueMap)
        {
            if (valueMap.TermUri != null)
            {
                return new ValueMappingProvider(valueMap.TermUri);
            }

            if (valueMap.NamespacePrefix != null && valueMap.TermName != null)
            {
                return new ValueMappingProvider(valueMap.NamespacePrefix,valueMap.TermName);
            }

            return new ValueMappingProvider();
        }

        private IEnumerable<IClassMappingProvider> GetClasses(EntityMap entityMap)
        {
            return entityMap.Classes.Select(c => c.Accept(this)).ToList();
        }

        private IEnumerable<IPropertyMappingProvider> GetProperties(EntityMap entityMap)
        {
            return entityMap.Properties.Select(p => p.Accept(this)).ToList();
        }
    }
}