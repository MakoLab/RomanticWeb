using System;
using RomanticWeb.Entities.ResultAggregations;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Mapping.Conventions
{
    public class SingleOrDefaultConvention:IPropertyConvention
    {
        public bool ShouldApply(IPropertyMapping target)
        {
            return (!target.Aggregation.HasValue)&&(target.ReturnType==target.ReturnType.FindItemType());
        }

        public void Apply(IPropertyMapping target)
        {
            target.Aggregation=Aggregation.SingleOrDefault;
        }
    }
}