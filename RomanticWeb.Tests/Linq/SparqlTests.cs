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
	using RomanticWeb.Mapping;
	using RomanticWeb.Mapping.Model;
	using RomanticWeb.Ontologies;

	[TestFixture]
	public class SparqlTests
	{
		public interface IPerson:IEntity
		{
			string FirstName { get; }
			List<IPerson> Knows { get; }
		}

		private IEntityFactory _entityFactory;
		private TripleStore _store;
		private Mock<ITypeMapping> _personTypeMappingMock;
		private Mock<IPropertyMapping> _firstNamePropertyMappingMock;
		private Mock<IPropertyMapping> _knowsPropertyMappingMock;
		private Mock<IMapping> _personMappingMock;
		private Mock<IMappingsRepository> _mappingsRepositoryMock;
		private Mock<IOntologyProvider> _ontologyProviderMock;

		[SetUp]
		public void Setup()
		{
			_store=new TripleStore();
			_store.LoadTestFile("TriplesWithLiteralSubjects.ttl");
			_personTypeMappingMock=new Mock<ITypeMapping>(MockBehavior.Strict);
			_personTypeMappingMock.SetupGet(typeMapping => typeMapping.Uri).Returns(new Uri("http://xmlns.com/foaf/0.1/Person"));
			_firstNamePropertyMappingMock=new Mock<IPropertyMapping>();
			_firstNamePropertyMappingMock.SetupGet(propertyMapping => propertyMapping.Uri).Returns(new Uri("http://xmlns.com/foaf/0.1/firstName"));
			_knowsPropertyMappingMock=new Mock<IPropertyMapping>();
			_knowsPropertyMappingMock.SetupGet(propertyMapping => propertyMapping.Uri).Returns(new Uri("http://xmlns.com/foaf/0.1/knows"));
			_personMappingMock=new Mock<IMapping>(MockBehavior.Strict);
			_personMappingMock.SetupGet(mapping => mapping.Type).Returns(_personTypeMappingMock.Object);
			_personMappingMock.Setup(mapping => mapping.PropertyFor("FirstName")).Returns(_firstNamePropertyMappingMock.Object);
			_personMappingMock.Setup(mapping => mapping.PropertyFor("Knows")).Returns(_knowsPropertyMappingMock.Object);
			_mappingsRepositoryMock=new Mock<IMappingsRepository>(MockBehavior.Strict);
			_mappingsRepositoryMock.Setup(repository => repository.MappingFor<IPerson>()).Returns(_personMappingMock.Object);
			_ontologyProviderMock=new Mock<IOntologyProvider>(MockBehavior.Strict);
			_ontologyProviderMock.SetupGet(provider => provider.Ontologies).Returns(
				new Ontology[] { new Ontology(
					new NamespaceSpecification("foaf","http://xmlns.com/foaf/0.1/"),
					new RdfClass("Person"),
					new DatatypeProperty("givenName"),
					new DatatypeProperty("familyName"),
					new ObjectProperty("knows")),
				new Ontology(
					new NamespaceSpecification("rdf","http://www.w3.org/1999/02/22-rdf-syntax-ns#"),
					new Property("type")) });
			_entityFactory=new EntityFactory(_mappingsRepositoryMock.Object,_ontologyProviderMock.Object,new TripleStoreTripleSourceFactory(_store));
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
            Assert.That(entities[0].AsDynamic().foaf.givenName, Is.EqualTo("Tomasz"));
            Assert.That(entities[0].AsDynamic().foaf.familyName, Is.EqualTo("Pluskiewicz"));
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
            Assert.That(entities[0].AsDynamic().foaf.givenName, Is.EqualTo("Tomasz"));
            Assert.That(entities[0].AsDynamic().foaf.familyName, Is.EqualTo("Pluskiewicz"));
		}

		[Test]
		[Repeat(5)]
		public void Selecting_entities_by_providing_subject_identifier_condition_test()
		{
			Entity entity=(from resources in _entityFactory.AsQueryable<Entity>()
						   where resources.Id==(EntityId)"http://magi/people/Karol"
						   select resources).FirstOrDefault();
			Assert.That(entity,Is.Not.Null);
			Assert.That(entity,Is.InstanceOf<Entity>());
            Assert.That(entity.AsDynamic().foaf.givenName, Is.EqualTo("Karol"));
            Assert.That(entity.AsDynamic().foaf.familyName, Is.EqualTo("Szczepański"));
		}

		[Test]
		[Repeat(5)]
		public void Selecting_entities_by_providing_entity_mapped_type_condition_test()
		{
			IList<Entity> entities=(from resources in _entityFactory.AsQueryable<Entity>()
									where resources is IPerson
									select resources).ToList();
			Assert.That(entities.Count,Is.EqualTo(3));
			Entity tomasz=entities.Where(item => item.Id==(EntityId)"http://magi/people/Tomasz").FirstOrDefault();
			Assert.That(tomasz,Is.Not.Null);
			Assert.That(tomasz,Is.InstanceOf<Entity>());
			Assert.That(tomasz.AsDynamic.foaf.givenName,Is.EqualTo("Tomasz"));
			Assert.That(tomasz.AsDynamic.foaf.familyName,Is.EqualTo("Pluskiewicz"));
			Entity karol=entities.Where(item => item.Id==(EntityId)"http://magi/people/Karol").FirstOrDefault();
			Assert.That(karol,Is.Not.Null);
			Assert.That(karol,Is.InstanceOf<Entity>());
            Assert.That(karol.AsDynamic().foaf.givenName, Is.EqualTo("Karol"));
			Assert.That(karol.AsDynamic().foaf.familyName,Is.EqualTo("Szczepański"));
		}

		[Test]
		[Repeat(5)]
		public void Selecting_entities_by_providing_nested_predicate_value_condition_test()
		{
			Entity entity=(from resources in _entityFactory.AsQueryable<Entity>()
						   where ((IList<Entity>)((OntologyAccessor)resources["foaf"])["knows"]).Any(item => item.Id==(EntityId)"http://magi/people/Tomasz")
						   select resources).FirstOrDefault();
			Assert.That(entity,Is.Not.Null);
			Assert.That(entity,Is.InstanceOf<Entity>());
			Assert.That(entity.AsDynamic.foaf.givenName,Is.EqualTo("Karol"));
			Assert.That(entity.AsDynamic.foaf.familyName,Is.EqualTo("Szczepański"));
		}
	}
}