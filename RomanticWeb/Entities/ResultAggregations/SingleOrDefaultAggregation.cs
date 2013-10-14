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
            var objectList=objects as IList<object>??objects.ToList();
            try
            {
                var aggregate=(IEnumerable<object>)_flatten.Aggregate(objectList);
                return aggregate.SingleOrDefault();
            }
            catch (System.InvalidOperationException)
            {
                throw new CardinalityException(1, objectList.Count);
            }
        }
    }
}