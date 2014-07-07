using System.Collections.Generic;
using System.Linq;

namespace RomanticWeb.Entities.ResultAggregations
{
    internal class FirstOrDefault : IResultAggregator
    {
        public Aggregation Aggregation
        {
            get
            {
                return Aggregation.FirstOrDefault;
            }
        }

        public object Aggregate(IEnumerable<object> objects)
        {
            return objects.FirstOrDefault();
        }
    }
}