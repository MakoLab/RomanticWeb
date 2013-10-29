using System;
using RomanticWeb.Entities;
using RomanticWeb.Entities.ResultAggregations;

namespace RomanticWeb
{
    internal class DynamicPropertyAggregate
    {
        private ProcessingOperation _aggregation=ProcessingOperation.Flatten;

        public DynamicPropertyAggregate(string propertyName)
        {
            var tokens=propertyName.Split('_');

            if (tokens.Length==1)
            {
                Name = tokens[0];
                IsValid = true;
            }
            else if (tokens.Length==2)
            {
                ProcessingOperation aggregationOption;
                if (Enum.TryParse(tokens[0],true,out aggregationOption))
                {
                    Aggregation=aggregationOption;
                    IsValid=true;
                }

                Name=tokens[1];
            }
        }

        public ProcessingOperation Aggregation
        {
            get
            {
                return _aggregation;
            }

            private set
            {
                _aggregation=value;
            }
        }

        public string Name { get; private set; }

        public bool IsValid { get; private set; }
    }
}