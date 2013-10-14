using System.Collections.Generic;
using System.Linq;

namespace RomanticWeb.Entities.ResultAggregations
{
    [ResultAggregationStrategy(AggregateOperation.First)]
    internal class FirstResultAggregation : IResultAggregationStrategy
    {
        public object Aggregate(IEnumerable<object> objects)
        {
            return objects.First();
        }
    }
}