using System.Collections.Generic;
using System.Linq;

namespace RomanticWeb.Entities.ResultAggregations
{
    internal class FirstResultProcessing : IResultProcessingStrategy
    {
        public ProcessingOperation Operation
        {
            get
            {
                return ProcessingOperation.First;
            }
        }

        public object Process(IEnumerable<object> objects)
        {
            return objects.First();
        }
    }
}