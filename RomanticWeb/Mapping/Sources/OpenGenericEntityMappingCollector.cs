using System;
using System.Collections.Generic;
using System.Reflection;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.Mapping.Visitors;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Mapping.Sources
{
    internal class OpenGenericEntityMappingCollector:IMappingProviderVisitor
    {
        private readonly Type[] _genricArguments;
        
        private IEntityMappingProvider _currentEntityMapping;

        public OpenGenericEntityMappingCollector(Type[] genricArguments)
        {
            _genricArguments=genricArguments;
        }

        internal IList<IClassMappingProvider> ClassMappingProviders { get; private set; }

        internal IList<IPropertyMappingProvider> PropertyMappingProviders { get; private set; }

        public void Visit(ICollectionMappingProvider collectionMappingProvider)
        {
            PropertyMappingProviders.Add(new CollectionMapping(collectionMappingProvider,_currentEntityMapping.EntityType.MakeGenericType(_genricArguments).FindProperty(collectionMappingProvider.PropertyInfo.Name)));
        }

        public void Visit(IPropertyMappingProvider propertyMappingProvider)
        {
            PropertyMappingProviders.Add(new PropertyMapping(propertyMappingProvider,_currentEntityMapping.EntityType.MakeGenericType(_genricArguments).FindProperty(propertyMappingProvider.PropertyInfo.Name)));
        }

        public void Visit(IDictionaryMappingProvider dictionaryMappingProvider)
        {
            PropertyMappingProviders.Add(new DictionaryMapping(dictionaryMappingProvider,_currentEntityMapping.EntityType.MakeGenericType(_genricArguments).FindProperty(dictionaryMappingProvider.PropertyInfo.Name)));
        }

        public void Visit(IClassMappingProvider classMappingProvider)
        {
            ClassMappingProviders.Add(classMappingProvider);
        }

        public void Visit(IEntityMappingProvider entityMappingProvider)
        {
            _currentEntityMapping=entityMappingProvider;
            PropertyMappingProviders=new List<IPropertyMappingProvider>();
            ClassMappingProviders=new List<IClassMappingProvider>();
        }

        private class PropertyMapping:IPropertyMappingProvider
        {
            private readonly IPropertyMappingProvider _inner;
            private readonly PropertyInfo _property;

            public PropertyMapping(IPropertyMappingProvider openGeneric,PropertyInfo property)
            {
                _inner=openGeneric;
                _property=property;
            }

            public Func<IOntologyProvider,Uri> GetTerm
            {
                get { return _inner.GetTerm; }
                set { _inner.GetTerm=value; }
                }

            public PropertyInfo PropertyInfo { get { return _property; } }

            public Type ConverterType { get; set; }

            public void Accept(IMappingProviderVisitor mappingProviderVisitor)
            {
                mappingProviderVisitor.Visit(this);
            }
        }

        private class DictionaryMapping:PropertyMapping,IDictionaryMappingProvider
        {
            private readonly IDictionaryMappingProvider _openGeneric;

            public DictionaryMapping(IDictionaryMappingProvider openGeneric,PropertyInfo property) :base(openGeneric,property)
            {
                _openGeneric=openGeneric;
            }

            public ITermMappingProvider Value { get { return _openGeneric.Value; } }

            public ITermMappingProvider Key { get { return _openGeneric.Key; } }
        }

        private class CollectionMapping:PropertyMapping,ICollectionMappingProvider
        {
            private readonly ICollectionMappingProvider _openGeneric;

            public CollectionMapping(ICollectionMappingProvider openGeneric,PropertyInfo property):base(openGeneric,property)
            {
                _openGeneric=openGeneric;
            }

            public StoreAs StoreAs
            {
                get { return _openGeneric.StoreAs; }
                set { _openGeneric.StoreAs=value; }
                }

            public Type ElementConverterType
                {
                get { return _openGeneric.ElementConverterType; }
                set { _openGeneric.ElementConverterType=value; }
            }
        }
    }
}