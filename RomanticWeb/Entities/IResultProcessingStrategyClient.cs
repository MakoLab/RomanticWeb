using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RomanticWeb.ComponentModel.Composition;
using RomanticWeb.Entities.ResultAggregations;

namespace RomanticWeb.Entities
{
    internal interface IResultProcessingStrategyClient
    {
        IDictionary<ProcessingOperation,IResultProcessingStrategy> ResultAggregations { get; }
    }

    internal static class ResultProcessingStrategyClientExtensions
    {
        private static IDictionary<ProcessingOperation,IResultProcessingStrategy> _resultAggregations;
        private static object _locker=new Object();

        internal static IDictionary<ProcessingOperation,IResultProcessingStrategy> GetResultAggregations(this IResultProcessingStrategyClient client)
        {
            lock (_locker)
            {
                if (_resultAggregations==null)
                {
                    _resultAggregations=new ConcurrentDictionary<ProcessingOperation,IResultProcessingStrategy>();
                    foreach (IResultProcessingStrategy resultProcessingStrategy in ContainerFactory.GetInstancesImplementing<IResultProcessingStrategy>())
                    {
                        object[] attributes=resultProcessingStrategy.GetType().GetCustomAttributes(typeof(ResultProcessingStrategyAttribute),true);
                        if (attributes.Length>0)
                        {
                            ResultProcessingStrategyAttribute resultProcessingStrategyAttribute=(ResultProcessingStrategyAttribute)attributes[0];
                            _resultAggregations[resultProcessingStrategyAttribute.Operation]=resultProcessingStrategy;
                        }
                    }
                }
            }

            return _resultAggregations;
        }
    }
}