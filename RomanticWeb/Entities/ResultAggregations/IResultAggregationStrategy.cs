using System.Collections.Generic;

namespace RomanticWeb.Entities.ResultAggregations
{
    public interface IResultAggregationStrategy
    {
        object Aggregate(IEnumerable<object> objects);
    }
}