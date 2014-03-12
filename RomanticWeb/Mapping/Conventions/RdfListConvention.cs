using System.Collections.Generic;
using RomanticWeb.Entities.ResultAggregations;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Mapping.Conventions
{
    public class RdfListConvention:ICollectionConvention
    {
        public bool ShouldApply(ICollectionMapping target)
        {
            return !target.Aggregation.HasValue
                && target.ReturnType.IsGenericType
                && target.ReturnType.GetGenericTypeDefinition()==typeof(IList<>);
        }

        public void Apply(ICollectionMapping target)
        {
            target.Aggregation=Aggregation.SingleOrDefault;
            target.StorageStrategy=StorageStrategyOption.RdfList;
        }
    }
}