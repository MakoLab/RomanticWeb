using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using RomanticWeb.dotNetRDF;
using RomanticWeb.Tests.Helpers;
using RomanticWeb.Tests.Stubs;
using VDS.RDF;

namespace RomanticWeb.Tests.Linq
{
	[TestFixture]
	public class SparqlTests
	{
		private IEntityFactory _entityFactory;
		private TripleStore _store;
		private Mock<IMappingProvider> _mappingsMock;
		private IMappingProvider _mappings;

		[SetUp]
		public void Setup()
		{
			_store=new TripleStore();
			_store.LoadTestFile("TriplesWithLiteralSubjects.ttl");
			_mappingsMock=new Mock<IMappingProvider>(MockBehavior.Strict);
			_mappings=_mappingsMock.Object;
			_entityFactory=new EntityFactory(_mappings,new StaticOntologyProvider(),new TripleStoreTripleSourceFactory(_store));
		}

		[Test]
		public void Selecting_entities_by_providing_single_literal_predicate_value_condition_from_pointed_ontology_test()
		{
			IList<Entity> entities=(from entity in _entityFactory.AsQueryable<Entity>()
						  where (string)((OntologyAccessor)entity["foaf"])["givenName"]=="Tomasz"
						  select entity).ToList();
			Assert.That(entities.Count,Is.EqualTo(1));
			Assert.That(entities[0],Is.Not.Null);
			Assert.That(entities[0],Is.InstanceOf<Entity>());
			Assert.That(entities[0].AsDynamic.foaf.givenName,Is.EqualTo("Tomasz"));
			Assert.That(entities[0].AsDynamic.foaf.familyName,Is.EqualTo("Pluskiewicz"));
		}

		[Test]
		public void Selecting_entities_by_providing_single_literal_predicate_value_condition_test()
		{
			IList<Entity> entities=(from entity in _entityFactory.AsQueryable<Entity>()
									where (string)entity["givenName"]=="Tomasz"
									select entity).ToList();
			Assert.That(entities.Count,Is.EqualTo(1));
			Assert.That(entities[0],Is.Not.Null);
			Assert.That(entities[0],Is.InstanceOf<Entity>());
			Assert.That(entities[0].AsDynamic.foaf.givenName,Is.EqualTo("Tomasz"));
			Assert.That(entities[0].AsDynamic.foaf.familyName,Is.EqualTo("Pluskiewicz"));
		}

		[Test]
		public void Selecting_entities_by_providing_subject_identifier_condition_test()
		{
			IList<Entity> entities=(from entity in _entityFactory.AsQueryable<Entity>()
									where entity.Id=="http://magi/people/Karol"
									select entity).ToList();
			Assert.That(entities.Count,Is.EqualTo(1));
			Assert.That(entities[0],Is.Not.Null);
			Assert.That(entities[0],Is.InstanceOf<Entity>());
			Assert.That(entities[0].AsDynamic.foaf.givenName,Is.EqualTo("Karol"));
			Assert.That(entities[0].AsDynamic.foaf.familyName,Is.EqualTo("Szczepański"));
		}
	}
}