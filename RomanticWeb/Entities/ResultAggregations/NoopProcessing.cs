using System.Collections.Generic;
using System.Linq;

namespace RomanticWeb.Entities.ResultAggregations
{
    internal class NoopProcessing:IResultProcessingStrategy
    {
        public ProcessingOperation Operation
        {
            get
            {
                return ProcessingOperation.Original;
            }
        }

        public object Process(IEnumerable<object> objects)
        {
            return objects.ToList();
        }
    }
}