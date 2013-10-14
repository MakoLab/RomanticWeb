using System.Collections.Generic;
using System.Linq;

namespace RomanticWeb.Entities.ResultAggregations
{
    [ResultAggregationStrategy(AggregateOperation.Original)]
    internal class NoopAggregation:IResultAggregationStrategy
    {
        public object Aggregate(IEnumerable<object> objects)
        {
            return objects.ToList();
        }
    }
}