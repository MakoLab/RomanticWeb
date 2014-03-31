using System;
using System.Collections.Generic;
using RomanticWeb.Converters;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Mapping.Providers;

namespace RomanticWeb.Mapping.Conventions
{
    public class DefaultConvertersConvention:IPropertyConvention
    {
        private readonly IDictionary<Type,Type> _defaultConverters;

        public DefaultConvertersConvention()
        {
            _defaultConverters=new Dictionary<Type,Type>();
        }

        public bool ShouldApply(IPropertyMappingProvider target)
        {
            var converterNotSet=target.ConverterType==null;
            var isKnownPropertyType=GetConverterType(target.PropertyInfo.PropertyType)!=null;
            return converterNotSet&&isKnownPropertyType;
        }

        public void Apply(IPropertyMappingProvider target)
        {
            var isRdfList = (target is ICollectionMappingProvider) && (target as ICollectionMappingProvider).StoreAs == StoreAs.RdfList;

            if (isRdfList)
            {
                target.ConverterType=typeof(AsEntityConverter<IEntity>);
            }
            else
            {
                target.ConverterType=GetConverterType(target.PropertyInfo.PropertyType);
            }
        }

        public DefaultConvertersConvention SetDefault<T,TConverter>()
            where TConverter:INodeConverter,new()
        {
            SetDefault(typeof(T), typeof(TConverter));
            return this;
        }

        public DefaultConvertersConvention SetDefault<TConverter>(params Type[] types)
            where TConverter:INodeConverter,new()
        {
            foreach (var type in types)
            {
                SetDefault(type,typeof(TConverter));
            }
            
            return this;
        }

        private void SetDefault(Type type,Type converterType)
        {
            _defaultConverters[type]=converterType;
        }

        private Type GetConverterType(Type propertyType)
        {
            if (!_defaultConverters.ContainsKey(propertyType))
            {
                propertyType=propertyType.FindItemType();
            }

            if (_defaultConverters.ContainsKey(propertyType))
            {
                return _defaultConverters[propertyType];
            }

            return null;
        }
    }
}