using System;
using NUnit.Framework;
using RomanticWeb.DotNetRDF;
using RomanticWeb.Tests.Helpers;
using VDS.RDF;

namespace RomanticWeb.Tests.IntegrationTests
{
    [TestFixture]
    public class InMemoryMappingTests:MappingTests
    {
        private TripleStore _store;

        protected TripleStore Store
        {
            get
            {
                if (_store==null)
                {
                    _store=new TripleStore();
                }

                return _store;
            }
        }

        protected override void LoadTestFile(string fileName)
        {
            Console.WriteLine("Reading dataset file '{0}'", fileName);
            Store.LoadTestFile(fileName);
        }

        protected override void LoadTestFile(string fileName, Uri graphUri)
        {
            Console.WriteLine("Reading dataset file '{0}' to graph {1}", fileName, graphUri);
            Store.LoadTestFile(fileName, graphUri);
        }

        protected override IEntitySource CreateEntitySource()
        {
            return new TripleStoreAdapter(Store);
        }

        protected override void ChildTeardown()
        {
            _store=null;
        }
    }
}