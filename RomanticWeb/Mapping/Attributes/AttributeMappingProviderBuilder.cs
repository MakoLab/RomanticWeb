using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RomanticWeb.Mapping.Providers;

namespace RomanticWeb.Mapping.Attributes
{
    // todo: refactor similarity with FluentMappingProviderBuilder
    internal class AttributeMappingProviderBuilder:Visitors.IMappingAttributesVisitor
    {
        private Type _entityType;

        public IClassMappingProvider Visit(ClassAttribute attribute)
        {
            if (attribute.Uri!=null)
            {
                return new ClassMappingProvider(_entityType,attribute.Uri);
            }

            return new ClassMappingProvider(_entityType,attribute.Prefix,attribute.Term);
        }

        public IPropertyMappingProvider Visit(PropertyAttribute propertyAttribute,PropertyInfo property)
        {
            return CreatePropertyMapping(propertyAttribute,property);
        }

        public ICollectionMappingProvider Visit(CollectionAttribute collectionAttribute,PropertyInfo property)
        {
            var prop=CreatePropertyMapping(collectionAttribute,property);
            return new CollectionMappingProvider(prop,collectionAttribute.StoreAs);
        }

        public IDictionaryMappingProvider Visit(DictionaryAttribute dictionaryAttribute,PropertyInfo property,ITermMappingProvider key,ITermMappingProvider value)
        {
            var prop=CreatePropertyMapping(dictionaryAttribute,property);
            return new DictionaryMappingProvider(prop,key,value);
        }

        public ITermMappingProvider Visit(KeyAttribute keyAttribute)
        {
            if (keyAttribute==null)
            {
                return new KeyMappingProvider();
            }

            if (keyAttribute.Uri != null)
            {
                return new KeyMappingProvider(keyAttribute.Uri);
            }

            return new KeyMappingProvider(keyAttribute.Prefix, keyAttribute.Term);
        }

        public ITermMappingProvider Visit(ValueAttribute valueAttribute)
        {
            if (valueAttribute==null)
            {
                return new ValueMappingProvider();
            }

            if (valueAttribute.Uri!=null)
            {
                return new ValueMappingProvider(valueAttribute.Uri);
            }
            
            return new ValueMappingProvider(valueAttribute.Prefix,valueAttribute.Term);
        }

        public IEntityMappingProvider Visit(Type entityType)
        {
            _entityType=entityType;
            return new EntityMappingProvider(entityType,GetClasses(entityType),GetProperties(entityType));
        }

        private static PropertyMappingProvider CreatePropertyMapping(PropertyAttribute propertyAttribute,PropertyInfo property)
        {
            PropertyMappingProvider propertyMappingProvider;
            if (propertyAttribute.Uri != null)
            {
                propertyMappingProvider=new PropertyMappingProvider(propertyAttribute.Uri,property);
            }
            else
            {
                propertyMappingProvider=new PropertyMappingProvider(propertyAttribute.Prefix,propertyAttribute.Term,property);
            }

            if (propertyAttribute.ConverterType != null)
            {
                propertyMappingProvider.ConverterType=propertyAttribute.ConverterType;
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