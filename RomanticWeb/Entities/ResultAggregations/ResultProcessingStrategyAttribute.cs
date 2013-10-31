using System;
using System.ComponentModel.Composition;

namespace RomanticWeb.Entities.ResultAggregations
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    internal class ResultProcessingStrategyAttribute : ExportAttribute
    {
        public ResultProcessingStrategyAttribute(ProcessingOperation operation)
            : base(typeof(IResultProcessingStrategy))
        {
            Operation=operation;
        }

        public ProcessingOperation Operation { get; private set; }
    }
}