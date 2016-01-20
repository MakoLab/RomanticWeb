using System;
using System.Collections.Generic;
using RomanticWeb.Converters;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Providers;

namespace RomanticWeb.Mapping.Conventions
{
    /// <summary>Sets the converter of <see cref="IEntity"/> properties to an appropriate.<see cref="AsEntityConverter{TEntityId}"/>
    /// </summary>
    public class EntityPropertiesConvention : IPropertyConvention
    {
        private static readonly IEnumerable<Type> Predecesors = new Type[0];

        /// <inheritdoc/>
        public IEnumerable<Type> Requires { get { return Predecesors; } }

        /// <inheritdoc/>
        public bool ShouldApply(IPropertyMappingProvider target)
        {
            return target.ConverterType == null
                   && typeof(IEntity).IsAssignableFrom(target.PropertyInfo.PropertyType.FindItemType());
        }

        /// <inheritdoc/>
        public void Apply(IPropertyMappingProvider target)
        {
            target.ConverterType = typeof(AsEntityConverter<>).MakeGenericType(target.PropertyInfo.PropertyType.FindItemType());
        }
    }
}