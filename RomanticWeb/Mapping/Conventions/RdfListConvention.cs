using System.Collections.Generic;
using RomanticWeb.Entities.ResultAggregations;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Mapping.Conventions
{
    public class RdfListConvention:IPropertyConvention
    {
        public bool ShouldApply(IPropertyMapping target)
        {
            return !target.Aggregation.HasValue
                && target.ReturnType.IsGenericType
                && target.ReturnType.GetGenericTypeDefinition()==typeof(IList<>);
        }

        public void Apply(IPropertyMapping target)
        {
            target.Aggregation=Aggregation.SingleOrDefault;
            target.StorageStrategy=StorageStrategyOption.RdfList;
        }
    }
}