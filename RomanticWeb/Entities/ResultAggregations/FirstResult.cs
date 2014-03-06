using System.Collections.Generic;
using System.Linq;

namespace RomanticWeb.Entities.ResultAggregations
{
    internal class FirstResult:IResultAggregator
    {
        public Aggregation Aggregation
        {
            get
            {
                return Aggregation.First;
            }
        }

        public object Aggregate(IEnumerable<object> objects)
        {
            return objects.First();
        }
    }
}