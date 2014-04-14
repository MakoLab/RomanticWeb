using System;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Tests.Stubs
{
    internal class TestPropertyMapping:PropertyMapping
    {
        public TestPropertyMapping(Type declaringType,Type returnType,string name,Uri predicateUri):base(declaringType,returnType,name,predicateUri)
        {
        }
    }
}