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

        public IPropertyMappingProvider Visit(PropertyInfo property,PropertyAttribute attribute)
        {
            if (attribute.Uri != null)
            {
                return new PropertyMappingProvider(attribute.Uri,property);
            }

            return new PropertyMappingProvider(attribute.Prefix,attribute.Term,property);
        }

        public ICollectionMappingProvider Visit(PropertyInfo property,CollectionAttribute attribute)
        {
            if (attribute.Uri != null)
            {
                return new CollectionMappingProvider(attribute.Uri,attribute.StoreAs,property);
            }

            return new CollectionMappingProvider(attribute.Prefix,attribute.Term,attribute.StoreAs,property);
        }

        public IDictionaryMappingProvider Visit(PropertyInfo property,DictionaryAttribute attribute,ITermMappingProvider key,ITermMappingProvider value)
        {
            if (attribute.Uri != null)
            {
                return new DictionaryMappingProvider(attribute.Uri,key,value,property);
            }

            return new DictionaryMappingProvider(attribute.Prefix,attribute.Term,key,value,property);
        }

        public ITermMappingProvider Visit(KeyAttribute attribute)
        {
            if (attribute==null)
            {
                return new KeyMappingProvider();
            }

            if (attribute.Uri != null)
            {
                return new KeyMappingProvider(attribute.Uri);
            }

            return new KeyMappingProvider(attribute.Prefix, attribute.Term);
        }

        public ITermMappingProvider Visit(ValueAttribute attribute)
        {
            if (attribute==null)
            {
                return new ValueMappingProvider();
            }

            if (attribute.Uri!=null)
            {
                return new ValueMappingProvider(attribute.Uri);
            }
            
            return new ValueMappingProvider(attribute.Prefix,attribute.Term);
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