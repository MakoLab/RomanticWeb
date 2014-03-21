using System;
using RomanticWeb.Entities.ResultAggregations;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Tests.Stubs
{
    internal class TestPropertyMapping : PropertyMapping,IPropertyMapping
	{
        public TestPropertyMapping(
            Type returnType,
            string name,
            Uri predicateUri)
            :base(returnType,name,predicateUri)
        {
        }
	}
}