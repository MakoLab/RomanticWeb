using System.Collections.Generic;
using System.Linq;

namespace RomanticWeb.Entities.ResultAggregations
{
    internal class SingleOrDefaultProcessing : IResultProcessingStrategy
    {
        private readonly ResultFlatteningProcessing _flatten=new ResultFlatteningProcessing();

        public ProcessingOperation Operation
        {
            get
            {
                return ProcessingOperation.SingleOrDefault;
            }
        }

        [Anotar.NLog.LogToErrorOnException]
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