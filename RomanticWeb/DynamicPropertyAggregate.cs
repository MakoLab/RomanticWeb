using System;
using RomanticWeb.Entities;

namespace RomanticWeb
{
    internal class DynamicPropertyAggregate
    {
        private AggregateOperation _aggregation=AggregateOperation.Flatten;

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
                AggregateOperation aggregationOption;
                if (Enum.TryParse(tokens[0],true,out aggregationOption))
                {
                    Aggregation=aggregationOption;
                    IsValid=true;
                }

                Name=tokens[1];
            }
        }

        public AggregateOperation Aggregation
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