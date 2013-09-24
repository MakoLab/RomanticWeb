using System;
using Moq;
using NUnit.Framework;
using RomanticWeb.DotNetRDF;
using RomanticWeb.Mapping;
using RomanticWeb.Tests.Helpers;
using RomanticWeb.Tests.Stubs;
using VDS.RDF;

namespace RomanticWeb.Tests.IntegrationTests
{
    public class InMemoryTripleStoreTestsBase
	{
		private TripleStore _store;

        private IMappingsRepository _mappings;

        public IMappingsRepository Mappings
        {
            get
            {
                return _mappings;
            }
        }

		protected IEntityFactory EntityFactory { get; private set; }

        [SetUp]
		public void Setup()
		{
			_store = new TripleStore();
			_mappings = SetupMappings();
            EntityFactory = new EntityContext(Mappings, new TestOntologyProvider(),new TripleStoreAdapter(_store));

			ChildSetup();
		}

		[TearDown]
		public void Teardown()
		{
			ChildTeardown();
		}

		protected virtual void ChildTeardown()
		{
		}

		protected virtual IMappingsRepository SetupMappings()
		{
			return new Mock<IMappingsRepository>(MockBehavior.Strict).Object;
		}

		protected virtual void ChildSetup()
		{
		}

		protected void LoadTestFile(string fileName, Uri graphUri=null)
		{
            if (graphUri!=null)
            {
                var graph=new Graph();
                graph.BaseUri=graphUri;
                graph.LoadTestFile(fileName);
                _store.Add(graph);
            }

		    _store.LoadTestFile(fileName);
		}
	}
}