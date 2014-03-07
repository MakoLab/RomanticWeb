using System;
using RomanticWeb.Entities.ResultAggregations;

namespace RomanticWeb.Dynamic
{
    internal class DynamicPropertyAggregate
    {
        private Aggregation _aggregation=Aggregation.Original;

        public DynamicPropertyAggregate(string propertyName)
        {
            var tokens=propertyName.Split('_');

            if (tokens.Length==1)
            {
                Name = tokens[0];
                IsValid=true;
            }
            else if (tokens.Length==2)
            {
                Aggregation aggregationOption;
                if (Enum.TryParse(tokens[0],true,out aggregationOption))
                {
                    Aggregation=aggregationOption;
                    IsValid=true;
                }
                else if (tokens[0].Equals("list",StringComparison.OrdinalIgnoreCase))
                {
                    Aggregation=Aggregation.SingleOrDefault;
                    IsValid=true;
                    IsList=true;
                }

                Name=tokens[1];
            }
        }

        public Aggregation Aggregation
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

        public bool IsList { get; private set; }
    }
}