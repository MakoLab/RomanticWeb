using System;
using System.Reflection;
using RomanticWeb.Entities.ResultAggregations;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Mapping.Fluent
{
    public abstract class PropertyMapBase<T> : TermMap, IPropertyMappingProvider
        where T : TermMap
    {
        private readonly PropertyInfo _propertyInfo;

        protected PropertyMapBase(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
        }

        /// <summary>Gets a predicate map part.</summary>
        public abstract ITermPart<T> Term { get; }

        /// <summary>Not used for property map.</summary>
        /// <returns><see cref="StorageStrategyOption.None"/></returns>
        /// <remarks>Setting throws <see cref="InvalidOperationException"/></remarks>
        protected internal virtual StorageStrategyOption StorageStrategy
        {
            get { return StorageStrategyOption.None; }

            set { throw new InvalidOperationException(); }
        }

        protected internal virtual Aggregation? Aggregation
        {
            get { return null; }

            set { throw new InvalidOperationException(); }
        }

        /// <summary>Gets the mapped property's <see cref="System.Reflection.PropertyInfo"/>.</summary>
        protected PropertyInfo PropertyInfo { get { return _propertyInfo; } }

        public abstract IPropertyMapping GetMapping(MappingContext mappingContext);
    }
}