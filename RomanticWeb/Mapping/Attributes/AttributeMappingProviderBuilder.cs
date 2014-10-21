using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RomanticWeb.Mapping.Providers;

namespace RomanticWeb.Mapping.Attributes
{
    // todo: refactor similarity with FluentMappingProviderBuilder
    internal class AttributeMappingProviderBuilder : Visitors.IMappingAttributesVisitor
    {
        private Type _entityType;

        public IClassMappingProvider Visit(ClassAttribute attribute)
        {
            if (attribute.Uri != null)
            {
                return new ClassMappingProvider(_entityType, attribute.Uri);
            }

            return new ClassMappingProvider(_entityType, attribute.Prefix, attribute.Term);
        }

        public IPropertyMappingProvider Visit(PropertyAttribute propertyAttribute, PropertyInfo property)
        {
            return CreatePropertyMapping(propertyAttribute, property);
        }

        public ICollectionMappingProvider Visit(CollectionAttribute collectionAttribute, PropertyInfo property)
        {
            var propertyMapping = CreatePropertyMapping(collectionAttribute, property);
            var result = new CollectionMappingProvider(propertyMapping, collectionAttribute.StoreAs);
            if (collectionAttribute.ElementConverterType != null)
            {
                result.ElementConverterType = collectionAttribute.ElementConverterType;
            }

            return result;
        }

        public IDictionaryMappingProvider Visit(DictionaryAttribute dictionaryAttribute, PropertyInfo property, IPredicateMappingProvider key, IPredicateMappingProvider value)
        {
            var prop = CreatePropertyMapping(dictionaryAttribute, property);
            return new DictionaryMappingProvider(prop, key, value);
        }

        public IPredicateMappingProvider Visit(KeyAttribute keyAttribute)
        {
            if (keyAttribute == null)
            {
                return new KeyMappingProvider();
            }

            var keyMappingProvider = keyAttribute.Uri != null 
                ? new KeyMappingProvider(keyAttribute.Uri) 
                : new KeyMappingProvider(keyAttribute.Prefix, keyAttribute.Term);

            if (keyAttribute.ConverterType != null)
            {
                keyMappingProvider.ConverterType = keyAttribute.ConverterType;
            }

            return keyMappingProvider;
        }

        public IPredicateMappingProvider Visit(ValueAttribute valueAttribute)
        {
            if (valueAttribute == null)
            {
                return new ValueMappingProvider();
            }

            var valueMappingProvider = valueAttribute.Uri != null
                ? new ValueMappingProvider(valueAttribute.Uri)
                : new ValueMappingProvider(valueAttribute.Prefix, valueAttribute.Term);

            if (valueAttribute.ConverterType != null)
            {
                valueMappingProvider.ConverterType = valueAttribute.ConverterType;
            }

            return valueMappingProvider;
        }

        public IEntityMappingProvider Visit(Type entityType)
        {
            _entityType = entityType;
            return new EntityMappingProvider(entityType, GetClasses(entityType), GetProperties(entityType));
        }

        private static PropertyMappingProvider CreatePropertyMapping(PropertyAttribute propertyAttribute, PropertyInfo property)
        {
            PropertyMappingProvider propertyMappingProvider;
            if (propertyAttribute.Uri != null)
            {
                propertyMappingProvider = new PropertyMappingProvider(propertyAttribute.Uri, property);
            }
            else
            {
                propertyMappingProvider = new PropertyMappingProvider(propertyAttribute.Prefix, propertyAttribute.Term, property);
            }

            if (propertyAttribute.ConverterType != null)
            {
                propertyMappingProvider.ConverterType = propertyAttribute.ConverterType;
            }

            return propertyMappingProvider;
        }

        private IList<IPropertyMappingProvider> GetProperties(Type entityType)
        {
            return (from property in entityType.GetProperties()
                    from attribute in property.GetCustomAttributes<PropertyAttribute>()
                    select attribute.Accept(this, property)).ToList();
        }

        private IList<IClassMappingProvider> GetClasses(Type entityType)
        {
            return entityType.GetCustomAttributes<ClassAttribute>().Select(a => a.Accept(this)).ToList();
        }
    }
}