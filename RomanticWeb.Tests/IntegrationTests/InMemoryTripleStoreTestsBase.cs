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
            get { return _mappings; }
        }

		protected IEntityFactory EntityFactory { get; private set; }

		[SetUp]
		public void Setup()
		{
			_store = new TripleStore();
			_mappings = SetupMappings();
			var tripleSourceFactory = new TripleStoreTripleSourceFactory(_store);
            EntityFactory = new EntityContext(Mappings, new TestOntologyProvider(), tripleSourceFactory);

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

		protected void LoadTestFile(string fileName)
		{
			_store.LoadTestFile(fileName);
		}
	}
}