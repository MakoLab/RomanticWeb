using System;
using System.ComponentModel.Composition;

namespace RomanticWeb.Entities.ResultAggregations
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    internal class ResultAggregationStrategy : ExportAttribute
    {
        public ResultAggregationStrategy(AggregateOperation operation)
            : base(typeof(IResultAggregationStrategy))
        {
            Operation=operation;
        }

        public AggregateOperation Operation { get; private set; }
    }
}