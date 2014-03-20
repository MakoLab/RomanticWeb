using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using RomanticWeb.Entities.ResultAggregations;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Fluent
{
    public abstract class PropertyMapBase<T>:PropertyMapBase
        where T:TermMap
    {
        protected PropertyMapBase(PropertyInfo propertyInfo)
            :base(propertyInfo)
        {
        }

        /// <summary>Gets a predicate map part.</summary>
        public abstract ITermPart<T> Term { get; }

        public abstract Aggregation? Aggregation { get; }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules","SA1402:FileMayOnlyContainASingleClass",Justification="Generic and non generic class in one file")]
    public abstract class PropertyMapBase:TermMap
    {
        private readonly PropertyInfo _propertyInfo;

        protected PropertyMapBase(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
        }

        internal PropertyInfo PropertyInfo
        {
            get
            {
                return _propertyInfo;
            }
        }

        public abstract IPropertyMappingProvider Accept(IFluentMapsVisitor fluentMapsVisitor);
    }
}