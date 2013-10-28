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
            Store.LoadTestFile(fileName);
        }

        protected override void LoadTestFile(string fileName,Uri graphUri)
        {
            Store.LoadTestFile(fileName,graphUri);
        }

        protected override IEntitySource CreateEntitySource()
        {
            return new TripleStoreAdapter(Store);
        }

        protected override void ChildTeardown()
        {
            _store = null;
        }
    }
}