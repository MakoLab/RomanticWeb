using System;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Tests.Stubs
{
    public class TestPropertyMapping : IPropertyMapping
	{
		public Uri Uri { get; set; }

		public GraphSelectionStrategyBase GraphSelector { get; set; }

        public string Name { get; set; }

        public bool IsCollection { get; set; }

        public Type ReturnType { get; set; }

        public StorageStrategyOption StorageStrategy { get; set; }
	}
}