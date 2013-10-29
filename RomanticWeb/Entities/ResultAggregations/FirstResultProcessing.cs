using System.Collections.Generic;
using System.Linq;

namespace RomanticWeb.Entities.ResultAggregations
{
    [ResultAggregationStrategy(ProcessingOperation.First)]
    internal class FirstResultProcessing : IResultProcessingStrategy
    {
        public object Process(IEnumerable<object> objects)
        {
            return objects.First();
        }
    }
}