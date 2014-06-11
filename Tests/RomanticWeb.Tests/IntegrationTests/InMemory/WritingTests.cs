using System;
using System.Linq;
using NUnit.Framework;
using RomanticWeb.DotNetRDF;
using RomanticWeb.Tests.Helpers;
using VDS.RDF;

namespace RomanticWeb.Tests.IntegrationTests.InMemory
{
    [TestFixture]
    public class WritingTests:WritingTestsBase
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

        protected override int MetagraphTripleCount
        {
            get
            {
                return _store[MetaGraphUri].Triples.Count;
            }
        }

        protected override int AllTriplesCount
        {
            get
            {
                return _store.Triples.Count();
            }
        }

        protected override void LoadTestFile(string fileName)
        {
            Console.WriteLine("Reading dataset file '{0}'",fileName);
            Store.LoadTestFile(fileName);
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