using System.Collections.Generic;
using System.Linq;

namespace RomanticWeb.Entities.ResultAggregations
{
    [ResultAggregationStrategy(AggregateOperation.SingleOrDefault)]
    internal class SingleOrDefaultAggregation : IResultAggregationStrategy
    {
        private readonly ResultFlatteningAggregation _flatten=new ResultFlatteningAggregation();

        public object Aggregate(IEnumerable<object> objects)
        {
            var objectList = objects as IList<object> ?? objects.ToList();
            var listFlattened = (IEnumerable<object>)_flatten.Aggregate(objectList);
            try
            {
                return listFlattened.SingleOrDefault();
            }
            catch (System.InvalidOperationException)
            {
                throw new CardinalityException(1, listFlattened.Count());
            }
        }
    }
}