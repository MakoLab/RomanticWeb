using System.Collections.Generic;
using System.Linq;

namespace RomanticWeb.Entities.ResultAggregations
{
    [ResultAggregationStrategy(AggregateOperation.Has)]
    internal class AnyResultCheckAggregation : IResultAggregationStrategy
    {
        public object Aggregate(IEnumerable<object> objects)
        {
            return objects.Any();
        }
    }
}