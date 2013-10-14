using System.Collections.Generic;
using System.Linq;

namespace RomanticWeb.Entities.ResultAggregations
{
    [ResultAggregationStrategy(AggregateOperation.FirstOrDefault)]
    internal class FirstOrDefaultAggregation : IResultAggregationStrategy
    {
        public object Aggregate(IEnumerable<object> objects)
        {
            return objects.FirstOrDefault();
        }
    }
}