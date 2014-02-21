using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        private static readonly object Locker = new Object();
        private static IDictionary<ProcessingOperation,IResultProcessingStrategy> resultAggregations;

        internal static IDictionary<ProcessingOperation,IResultProcessingStrategy> GetResultAggregations(this IResultProcessingStrategyClient client)
        {
            lock (Locker)
            {
                if (resultAggregations==null)
                {
                    resultAggregations=new ConcurrentDictionary<ProcessingOperation,IResultProcessingStrategy>();
                    foreach (IResultProcessingStrategy resultProcessingStrategy in ContainerFactory.GetInstancesImplementing<IResultProcessingStrategy>())
                    {
                        resultAggregations[resultProcessingStrategy.Operation] = resultProcessingStrategy;
                    }
                }
            }

            return resultAggregations;
        }
    }
}