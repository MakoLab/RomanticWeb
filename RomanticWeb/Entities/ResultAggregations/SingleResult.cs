using System.Collections.Generic;
using System.Linq;

namespace RomanticWeb.Entities.ResultAggregations
{
    internal class SingleResult:IResultAggregator
    {
        public Aggregation Aggregation
        {
            get
            {
                return Aggregation.Single;
            }
        }

        public object Aggregate(IEnumerable<object> objects)
        {
            return objects.Single();
        }
    }
}