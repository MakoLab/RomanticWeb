using System.Collections.Generic;
using RomanticWeb.Entities.ResultAggregations;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Mapping.Conventions
{
    public class CollectionConvention:IPropertyConvention
    {
        public bool ShouldApply(IPropertyMapping target)
        {
            return (!target.Aggregation.HasValue)&& target.ReturnType.IsGenericType 
                   && (target.ReturnType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                       || target.ReturnType.GetGenericTypeDefinition() == typeof(ICollection<>));
        }

        public void Apply(IPropertyMapping target)
        {
            target.Aggregation=Aggregation.Original;
            target.StorageStrategy=StorageStrategyOption.Simple;
        }
    }
}