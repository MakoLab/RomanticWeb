using System;
using RomanticWeb.Entities.ResultAggregations;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Tests.Stubs
{
    public class TestPropertyMapping : IPropertyMapping
	{
        public Uri Uri { get; set; }

        public string Name { get; set; }

        public Type ReturnType { get; set; }

        public StorageStrategyOption StorageStrategy { get; set; }

        public Aggregation? Aggregation { get; set; }
	}
}