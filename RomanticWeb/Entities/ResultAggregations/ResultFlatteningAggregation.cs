using System.Collections.Generic;
using System.Linq;

namespace RomanticWeb.Entities.ResultAggregations
{
    [ResultAggregationStrategy(AggregateOperation.Flatten)]
    internal class ResultFlatteningAggregation:IResultAggregationStrategy
    {
        public object Aggregate(IEnumerable<object> objects)
        {
            return FlattenResults(objects).ToList();
        }

        private static IEnumerable<object> FlattenResults(IEnumerable<object> arg)
        {
            foreach (var o in arg)
            {
                if (o is IEnumerable<object>)
                {
                    foreach (var result in FlattenResults((IEnumerable<object>)o))
                    {
                        yield return result;
                    }
                }
                else
                {
                    yield return o;
                }
            }
        }
    }
}