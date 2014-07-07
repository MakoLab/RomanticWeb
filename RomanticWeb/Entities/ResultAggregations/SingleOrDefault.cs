using System.Collections.Generic;
using System.Linq;

namespace RomanticWeb.Entities.ResultAggregations
{
    internal class SingleOrDefault : IResultAggregator
    {
        public Aggregation Aggregation
        {
            get
            {
                return Aggregation.SingleOrDefault;
            }
        }

        public object Aggregate(IEnumerable<object> objects)
        {
            var objectList = objects as IList<object> ?? objects.ToList();
            try
            {
                return objectList.SingleOrDefault();
            }
            catch (System.InvalidOperationException)
            {
                throw new CardinalityException(1, objectList.Count());
            }
        }
    }
}