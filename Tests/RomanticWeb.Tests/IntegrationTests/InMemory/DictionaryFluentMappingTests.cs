using System;
using NUnit.Framework;
using RomanticWeb.Mapping;
using RomanticWeb.TestEntities.FluentMappings;
using RomanticWeb.Tests.Helpers;

namespace RomanticWeb.Tests.IntegrationTests.InMemory
{
    [TestFixture]
    public class DictionaryFluentMappingTests : DictionaryTestsBase
    {
        protected override void LoadTestFile(string fileName)
        {
            Console.WriteLine("Reading dataset file '{0}'", fileName);
            Store.LoadTestFile(fileName);
        }

        protected override void BuildMappings(MappingBuilder m)
        {
            m.Fluent.FromAssemblyOf<AnimalMap>();
            m.AddMapping(GetType().Assembly, Mappings);
        }
    }
}