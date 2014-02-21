using System.Collections.Generic;
using System.Linq;

namespace RomanticWeb.Entities.ResultAggregations
{
    internal class FirstOrDefaultProcessing : IResultProcessingStrategy
    {
        public ProcessingOperation Operation
        {
            get
            {
                return ProcessingOperation.FirstOrDefault;
            }
        }

        public object Process(IEnumerable<object> objects)
        {
            return objects.FirstOrDefault();
        }
    }
}