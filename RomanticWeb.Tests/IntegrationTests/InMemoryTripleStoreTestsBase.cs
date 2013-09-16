using Moq;
using NUnit.Framework;
using RomanticWeb.Tests.Helpers;
using RomanticWeb.Tests.Stubs;
using RomanticWeb.DotNetRDF;
using VDS.RDF;

namespace RomanticWeb.Tests.IntegrationTests
{
    using RomanticWeb.Mapping;

    public class InMemoryTripleStoreTestsBase
	{
		private TripleStore _store;

		private IMappingsRepository _mappings;

		protected IEntityFactory EntityFactory { get; set; }

		public IMappingsRepository Mappings
		{
			get { return _mappings; }
		}

		[SetUp]
		public void Setup()
		{
			_store = new TripleStore();
			_mappings = SetupMappings();
			var tripleSourceFactory = new TripleStoreTripleSourceFactory(_store);
			EntityFactory = new EntityFactory(Mappings, new StaticOntologyProvider(), tripleSourceFactory);

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