using System;
using NUnit.Framework;
using RomanticWeb.Tests.Helpers;

namespace RomanticWeb.Tests.IntegrationTests.InMemory
{
    [TestFixture]
    public class MappingTests : MappingTestsBase
    {
        protected override void LoadTestFile(string fileName)
        {
            Console.WriteLine("Reading dataset file '{0}'", fileName);
            Store.LoadTestFile(fileName);
        }
    }
}