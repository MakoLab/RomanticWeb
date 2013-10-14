using System;
using System.Collections.Generic;
using System.Linq;

namespace RomanticWeb.Entities.ResultAggregations
{
    [ResultAggregationStrategy(AggregateOperation.Single)]
    internal class SingleResultAggregation : IResultAggregationStrategy
    {
        public object Aggregate(IEnumerable<object> objects)
        {
            return objects.Single();
        }
    }
}