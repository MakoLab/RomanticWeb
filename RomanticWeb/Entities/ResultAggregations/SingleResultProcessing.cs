using System;
using System.Collections.Generic;
using System.Linq;

namespace RomanticWeb.Entities.ResultAggregations
{
    internal class SingleResultProcessing : IResultProcessingStrategy
    {
        public ProcessingOperation Operation
        {
            get
            {
                return ProcessingOperation.Single;
            }
        }

        public object Process(IEnumerable<object> objects)
        {
            return objects.Single();
        }
    }
}