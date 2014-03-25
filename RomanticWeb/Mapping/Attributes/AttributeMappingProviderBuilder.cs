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
        public IClassMappingProvider Visit(ClassAttribute attribute)
        {
            if (attribute.Uri!=null)
            {
                return new ClassMappingProvider(attribute.Uri);
            }

            return new ClassMappingProvider(attribute.Prefix,attribute.Term);
        }

        public IPropertyMappingProvider Visit(PropertyAttribute propertyAttribute,PropertyInfo property)
        {
            if (propertyAttribute.Uri != null)
            {
                return new PropertyMappingProvider(propertyAttribute.Uri,property);
            }

            return new PropertyMappingProvider(propertyAttribute.Prefix,propertyAttribute.Term,property);
        }

        public ICollectionMappingProvider Visit(CollectionAttribute collectionAttribute,PropertyInfo property)
        {
            if (collectionAttribute.Uri != null)
            {
                return new CollectionMappingProvider(collectionAttribute.Uri,collectionAttribute.StoreAs,property);
            }

            return new CollectionMappingProvider(collectionAttribute.Prefix,collectionAttribute.Term,collectionAttribute.StoreAs,property);
        }

        public IDictionaryMappingProvider Visit(DictionaryAttribute dictionaryAttribute,PropertyInfo property,ITermMappingProvider key,ITermMappingProvider value)
        {
            if (dictionaryAttribute.Uri != null)
            {
                return new DictionaryMappingProvider(dictionaryAttribute.Uri,key,value,property);
            }

            return new DictionaryMappingProvider(dictionaryAttribute.Prefix,dictionaryAttribute.Term,key,value,property);
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
            return new EntityMappingProvider(entityType, GetClasses(entityType), GetProperties(entityType));
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