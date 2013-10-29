using System.Collections.Generic;
using System.Linq;

namespace RomanticWeb.Entities.ResultAggregations
{
    [ResultAggregationStrategy(ProcessingOperation.Has)]
    internal class AnyResultCheckProcessing : IResultProcessingStrategy
    {
        public object Process(IEnumerable<object> objects)
        {
            return objects.Any();
        }
    }
}