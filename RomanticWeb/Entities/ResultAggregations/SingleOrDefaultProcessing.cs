using System.Collections.Generic;
using System.Linq;

namespace RomanticWeb.Entities.ResultAggregations
{
    [ResultProcessingStrategy(ProcessingOperation.SingleOrDefault)]
    internal class SingleOrDefaultProcessing : IResultProcessingStrategy
    {
        private readonly ResultFlatteningProcessing _flatten=new ResultFlatteningProcessing();

        public object Process(IEnumerable<object> objects)
        {
            var objectList = objects as IList<object> ?? objects.ToList();
            var listFlattened = (IEnumerable<object>)_flatten.Process(objectList);
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