using System;
using System.Collections.Generic;
using System.Linq;

namespace RomanticWeb.Entities.ResultAggregations
{
    [ResultAggregationStrategy(ProcessingOperation.Single)]
    internal class SingleResultProcessing : IResultProcessingStrategy
    {
        public object Process(IEnumerable<object> objects)
        {
            return objects.Single();
        }
    }
}