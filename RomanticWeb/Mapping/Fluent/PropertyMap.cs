using System.Reflection;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Fluent
{
    /// <summary>A mapping definition for properties.</summary>
    public sealed class PropertyMap:PropertyMapBase<PropertyMap>
    {
        public PropertyMap(PropertyInfo propertyInfo)
            :base(propertyInfo)
        {
        }

        public override ITermPart<PropertyMap> Term
        {
            get
            {
                return new TermPart<PropertyMap>(this);
            }
        }

        public override Entities.ResultAggregations.Aggregation? Aggregation
        {
            get
            {
                return Entities.ResultAggregations.Aggregation.SingleOrDefault;
            }
        }

        public override IPropertyMappingProvider Accept(IFluentMapsVisitor fluentMapsVisitor)
        {
            return fluentMapsVisitor.Visit(this);
        }
    }
}