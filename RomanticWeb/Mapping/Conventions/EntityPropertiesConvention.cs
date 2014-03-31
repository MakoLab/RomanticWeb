using System;
using RomanticWeb.Converters;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Providers;

namespace RomanticWeb.Mapping.Conventions
{
    public class EntityPropertiesConvention:IPropertyConvention
    {
        public bool ShouldApply(IPropertyMappingProvider target)
        {
            return target.ConverterType == null && typeof(IEntity).IsAssignableFrom(target.PropertyInfo.PropertyType.FindItemType());
        }

        public void Apply(IPropertyMappingProvider target)
        {
            target.ConverterType = typeof(AsEntityConverter<>).MakeGenericType(target.PropertyInfo.PropertyType.FindItemType());
        }
    }
}