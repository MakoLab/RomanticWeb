using System.Collections.Generic;
using RomanticWeb.Entities.ResultAggregations;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Mapping.Conventions
{
    public class CollectionConvention:ICollectionConvention
    {
        public bool ShouldApply(ICollectionMapping target)
        {
            return (!target.Aggregation.HasValue)&& target.ReturnType.IsGenericType 
                   && (target.ReturnType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                       || target.ReturnType.GetGenericTypeDefinition() == typeof(ICollection<>));
        }

        public void Apply(ICollectionMapping target)
        {
            target.Aggregation=Aggregation.Original;
            target.StorageStrategy=StorageStrategyOption.Simple;
        }
    }
}