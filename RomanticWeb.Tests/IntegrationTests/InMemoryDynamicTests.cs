using System;
using NUnit.Framework;
using RomanticWeb.DotNetRDF;
using RomanticWeb.Tests.Helpers;
using VDS.RDF;

namespace RomanticWeb.Tests.IntegrationTests
{
    [TestFixture]
    public class InMemoryDynamicTests:DynamicTests
    {
        private TripleStore _store;

        protected override void LoadTestFile(string fileName)
        {
            _store.LoadTestFile(fileName);
        }

        protected override void LoadTestFile(string fileName,Uri graphUri)
        {
            _store.LoadTestFile(fileName,graphUri);
        }

        protected override IEntitySource CreateEntitySource()
        {
            _store=new TripleStore();
            return new TripleStoreAdapter(_store);
        }
    }
}