using System.Collections.Generic;

namespace RomanticWeb.Entities.ResultAggregations
{
    ////[ResultAggregationStrategy(AggregateOperation.Flatten)]
    internal class SingleOrDefaultNonCollectionAggregation : IResultAggregationStrategy
    {
        private readonly ResultFlatteningAggregation _flatten;

        private readonly SingleOrDefaultAggregation _singleOrDefault;

        public SingleOrDefaultNonCollectionAggregation()
        {
            _flatten=new ResultFlatteningAggregation();
            _singleOrDefault=new SingleOrDefaultAggregation();
        }

        public object Aggregate(IEnumerable<object> objects)
        {
            var result = _singleOrDefault.Aggregate(objects);

            return _flatten.Aggregate((IEnumerable<object>)result);
        }
    }
}