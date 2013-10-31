using System.Collections.Generic;
using System.Linq;

namespace RomanticWeb.Entities.ResultAggregations
{
    [ResultProcessingStrategy(ProcessingOperation.Original)]
    internal class NoopProcessing:IResultProcessingStrategy
    {
        public object Process(IEnumerable<object> objects)
        {
            return objects.ToList();
        }
    }
}