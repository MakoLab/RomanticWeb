using System.Collections.Generic;
using System.Linq;

namespace RomanticWeb.Entities.ResultAggregations
{
    internal class AnyResultCheckProcessing : IResultProcessingStrategy
    {
        public ProcessingOperation Operation
        {
            get
            {
                return ProcessingOperation.Has;
            }
        }

        public object Process(IEnumerable<object> objects)
        {
            return objects.Any();
        }
    }
}