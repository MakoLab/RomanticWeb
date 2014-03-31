using System;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Tests.Stubs
{
    internal class TestPropertyMapping:PropertyMapping
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