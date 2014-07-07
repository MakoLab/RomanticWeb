using System.Collections.Generic;
using System.Linq;

namespace RomanticWeb.Entities.ResultAggregations
{
    internal class AnyResultCheck : IResultAggregator
    {
        public Aggregation Aggregation
        {
            get
            {
                return Aggregation.Has;
            }
        }

        public object Aggregate(IEnumerable<object> objects)
        {
            return objects.Any();
        }
    }
}