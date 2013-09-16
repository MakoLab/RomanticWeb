using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using RomanticWeb.DotNetRDF;
using RomanticWeb.Tests.Helpers;
using RomanticWeb.Tests.Stubs;
using VDS.RDF;

namespace RomanticWeb.Tests.Linq
{
	[TestFixture]
	public class SparqlTests
	{
		public interface IPerson:IEntity
		{
			string FirstName { get; }
		}

		private IEntityFactory _entityFactory;
		private TripleStore _store;
		private Mock<ITypeMapping> _personTypeMappingMock;
		private Mock<IPropertyMapping> _firstNamePropertyMappingMock;
		private Mock<IMapping> _personMappingMock;
		private Mock<IMappingsRepository> _mappingsMock;
		private IMappingsRepository _mappings;

		[SetUp]
		public void Setup()
		{
			_store=new TripleStore();
			_store.LoadTestFile("TriplesWithLiteralSubjects.ttl");
			_personTypeMappingMock=new Mock<ITypeMapping>();
			_personTypeMappingMock.SetupGet(typeMapping => typeMapping.Uri).Returns(new Uri("http://xmlns.com/foaf/0.1/Person"));
			_firstNamePropertyMappingMock=new Mock<IPropertyMapping>();
			_firstNamePropertyMappingMock.SetupGet(propertyMapping => propertyMapping.Uri).Returns(new Uri("http://xmlns.com/foaf/0.1/firstName"));
			_personMappingMock=new Mock<IMapping>();
			_personMappingMock.SetupGet(mapping => mapping.Type).Returns(_personTypeMappingMock.Object);
			_personMappingMock.Setup(mapping => mapping.PropertyFor("FirstName")).Returns(_firstNamePropertyMappingMock.Object);
			_mappingsMock = new Mock<IMappingsRepository>(MockBehavior.Strict);
			_mappingsMock.Setup(mapping => mapping.MappingFor<IPerson>()).Returns(_personMappingMock.Object);
			_mappings=_mappingsMock.Object;
			_entityFactory=new EntityFactory(_mappings,new StaticOntologyProvider(),new TripleStoreTripleSourceFactory(_store));
		}

		[Test]
		[Repeat(5)]
		public void Selecting_entities_by_providing_single_literal_predicate_value_condition_from_pointed_ontology_test()
		{
			IList<Entity> entities=(from resources in _entityFactory.AsQueryable<Entity>()
									where (string)((OntologyAccessor)resources["foaf"])["givenName"]=="Tomasz"
									select resources).ToList();
			Assert.That(entities.Count,Is.EqualTo(1));
			Assert.That(entities[0],Is.Not.Null);
			Assert.That(entities[0],Is.InstanceOf<Entity>());
			Assert.That(entities[0].AsDynamic.foaf.givenName,Is.EqualTo("Tomasz"));
			Assert.That(entities[0].AsDynamic.foaf.familyName,Is.EqualTo("Pluskiewicz"));
		}

		[Test]
		[Repeat(5)]
		public void Selecting_entities_by_providing_single_literal_predicate_value_condition_test()
		{
			IList<Entity> entities=(from resources in _entityFactory.AsQueryable<Entity>()
									where (string)resources["givenName"]=="Tomasz"
									select resources).ToList();
			Assert.That(entities.Count,Is.EqualTo(1));
			Assert.That(entities[0],Is.Not.Null);
			Assert.That(entities[0],Is.InstanceOf<Entity>());
			Assert.That(entities[0].AsDynamic.foaf.givenName,Is.EqualTo("Tomasz"));
			Assert.That(entities[0].AsDynamic.foaf.familyName,Is.EqualTo("Pluskiewicz"));
		}

		[Test]
		[Repeat(5)]
		public void Selecting_entities_by_providing_subject_identifier_condition_test()
		{
			Entity entity=(from resources in _entityFactory.AsQueryable<Entity>()
						   where resources.Id=="http://magi/people/Karol"
						   select resources).FirstOrDefault();
			Assert.That(entity,Is.Not.Null);
			Assert.That(entity,Is.InstanceOf<Entity>());
			Assert.That(entity.AsDynamic.foaf.givenName,Is.EqualTo("Karol"));
			Assert.That(entity.AsDynamic.foaf.familyName,Is.EqualTo("Szczepański"));
		}
		
		[Repeat(5)]
		[Test]
		public void Selecting_entities_by_providing_entity_mapped_type_condition_test()
		{
			IList<Entity> entities=(from resources in _entityFactory.AsQueryable<Entity>()
									where resources is IPerson
									select resources).ToList();
			Assert.That(entities.Count,Is.EqualTo(2));
			Assert.That(entities[0],Is.Not.Null);
			Assert.That(entities[0],Is.InstanceOf<Entity>());
			Assert.That(entities[0].AsDynamic.foaf.givenName,Is.EqualTo("Tomasz"));
			Assert.That(entities[0].AsDynamic.foaf.familyName,Is.EqualTo("Pluskiewicz"));
			Assert.That(entities[1],Is.Not.Null);
			Assert.That(entities[1],Is.InstanceOf<Entity>());
			Assert.That(entities[1].AsDynamic.foaf.givenName,Is.EqualTo("Karol"));
			Assert.That(entities[1].AsDynamic.foaf.familyName,Is.EqualTo("Szczepański"));
		}
	}
}