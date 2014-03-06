using System.Collections.Generic;
using System.Linq;

namespace RomanticWeb.Entities.ResultAggregations
{
    internal class OriginalResult:IResultAggregator
    {
        public Aggregation Aggregation
        {
            get
            {
                return Aggregation.Original;
            }
        }

        public object Aggregate(IEnumerable<object> objects)
        {
            return objects.ToList();
        }
    }
}