using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using RomanticWeb.DotNetRDF;
using RomanticWeb.Entities;
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
			string FamilyName { get; }
			List<IPerson> Knows { get; }
		}

		private IEntityContext _entityContext;
		private TripleStore _store;
		private Mock<IClassMapping> _personTypeMappingMock;
		private Mock<IPropertyMapping> _firstNamePropertyMappingMock;
		private Mock<IPropertyMapping> _knowsPropertyMappingMock;
		private Mock<IEntityMapping> _personMappingMock;
		private Mock<IMappingsRepository> _mappingsRepositoryMock;
		private Mock<IOntologyProvider> _ontologyProviderMock;

		[SetUp]
		public void Setup()
		{
			_store=new TripleStore();
			_store.LoadTestFile("TriplesWithLiteralSubjects.trig");
			_personTypeMappingMock=new Mock<IClassMapping>(MockBehavior.Strict);
			_personTypeMappingMock.SetupGet(typeMapping => typeMapping.Uri).Returns(new Uri("http://xmlns.com/foaf/0.1/Person"));
			_firstNamePropertyMappingMock=new Mock<IPropertyMapping>();
			_firstNamePropertyMappingMock.SetupGet(propertyMapping => propertyMapping.Uri).Returns(new Uri("http://xmlns.com/foaf/0.1/familyName"));
			_knowsPropertyMappingMock=new Mock<IPropertyMapping>();
			_knowsPropertyMappingMock.SetupGet(propertyMapping => propertyMapping.Uri).Returns(new Uri("http://xmlns.com/foaf/0.1/knows"));
			_personMappingMock=new Mock<IEntityMapping>(MockBehavior.Strict);
			_personMappingMock.SetupGet(mapping => mapping.Class).Returns(_personTypeMappingMock.Object);
			_personMappingMock.Setup(mapping => mapping.PropertyFor("FamilyName")).Returns(_firstNamePropertyMappingMock.Object);
			_personMappingMock.Setup(mapping => mapping.PropertyFor("Knows")).Returns(_knowsPropertyMappingMock.Object);
			_mappingsRepositoryMock=new Mock<IMappingsRepository>(MockBehavior.Strict);
		    _mappingsRepositoryMock.Setup(m => m.RebuildMappings(It.IsAny<IOntologyProvider>()));
			_mappingsRepositoryMock.Setup(repository => repository.MappingFor<IPerson>()).Returns(_personMappingMock.Object);
			_ontologyProviderMock=new Mock<IOntologyProvider>(MockBehavior.Strict);
			_ontologyProviderMock.SetupGet(provider => provider.Ontologies).Returns(
				new Ontology[] { new Ontology(
					new NamespaceSpecification("foaf","http://xmlns.com/foaf/0.1/"),
					new Class("Person"),
					new DatatypeProperty("givenName"),
					new DatatypeProperty("familyName"),
					new ObjectProperty("knows")),
				new Ontology(
					new NamespaceSpecification("rdf","http://www.w3.org/1999/02/22-rdf-syntax-ns#"),
					new Property("type")) });
			_entityContext=new EntityContext(_mappingsRepositoryMock.Object,new TripleStoreAdapter(_store))
			                   {
                                   OntologyProvider = _ontologyProviderMock.Object
			                   };
		}

		[Test]
		[Repeat(5)]
		public void Selecting_entities_by_providing_single_literal_predicate_value_condition_from_pointed_ontology_test()
		{
			IList<IEntity> entities=(from resources in _entityContext.AsQueryable<IEntity>()
									where (string)((OntologyAccessor)resources["foaf"])["givenName"]=="Tomasz"
									select resources).ToList();
			Assert.That(entities.Count,Is.EqualTo(1));
			Assert.That(entities[0],Is.Not.Null);
			Assert.That(entities[0],Is.InstanceOf<IEntity>());
            Assert.That(entities[0].AsDynamic().foaf.givenName, Is.EqualTo("Tomasz"));
            Assert.That(entities[0].AsDynamic().foaf.familyName, Is.EqualTo("Pluskiewicz"));
		}

		[Test]
		[Repeat(5)]
		public void Selecting_entities_by_providing_single_literal_predicate_value_condition_test()
		{
			IList<IEntity> entities=(from resources in _entityContext.AsQueryable<IEntity>()
									where (string)resources["givenName"]=="Tomasz"
									select resources).ToList();
			Assert.That(entities.Count,Is.EqualTo(1));
			Assert.That(entities[0],Is.Not.Null);
			Assert.That(entities[0],Is.InstanceOf<IEntity>());
            Assert.That(entities[0].AsDynamic().foaf.givenName, Is.EqualTo("Tomasz"));
            Assert.That(entities[0].AsDynamic().foaf.familyName, Is.EqualTo("Pluskiewicz"));
		}

		[Test]
		[Repeat(5)]
		public void Selecting_entities_by_providing_subject_identifier_condition_test()
		{
			IEntity entity=(from resources in _entityContext.AsQueryable<IEntity>()
						   where resources.Id==(EntityId)"http://magi/people/Karol"
						   select resources).FirstOrDefault();
			Assert.That(entity,Is.Not.Null);
			Assert.That(entity,Is.InstanceOf<IEntity>());
            Assert.That(entity.AsDynamic().foaf.givenName, Is.EqualTo("Karol"));
            Assert.That(entity.AsDynamic().foaf.familyName, Is.EqualTo("Szczepański"));
		}

		[Test]
		[Repeat(5)]
		public void Selecting_entities_by_providing_entity_mapped_type_condition_test()
		{
			IList<IEntity> entities=(from resources in _entityContext.AsQueryable<IEntity>()
									where resources is IPerson
									select resources).ToList();
			Assert.That(entities.Count,Is.EqualTo(3));
			IEntity tomasz=entities.Where(item => item.Id==(EntityId)"http://magi/people/Tomasz").FirstOrDefault();
			Assert.That(tomasz,Is.Not.Null);
			Assert.That(tomasz,Is.InstanceOf<Entity>());
			Assert.That(tomasz.AsDynamic().foaf.givenName,Is.EqualTo("Tomasz"));
			Assert.That(tomasz.AsDynamic().foaf.familyName,Is.EqualTo("Pluskiewicz"));
			IEntity karol=entities.Where(item => item.Id==(EntityId)"http://magi/people/Karol").FirstOrDefault();
			Assert.That(karol,Is.Not.Null);
			Assert.That(karol,Is.InstanceOf<Entity>());
            Assert.That(karol.AsDynamic().foaf.givenName, Is.EqualTo("Karol"));
			Assert.That(karol.AsDynamic().foaf.familyName,Is.EqualTo("Szczepański"));
		}

		[Test]
		[Repeat(5)]
		public void Selecting_specific_type_entities_test()
		{
			IList<IPerson> entities=(from resources in _entityContext.AsQueryable<IPerson>()
									 select resources).ToList();
			Assert.That(entities.Count,Is.EqualTo(3));
			IPerson tomasz=entities.Where(item => item.Id==(EntityId)"http://magi/people/Tomasz").FirstOrDefault();
			Assert.That(tomasz,Is.Not.Null);
			Assert.That(tomasz,Is.InstanceOf<IEntity>());
			Assert.That(tomasz.FamilyName,Is.EqualTo("Pluskiewicz"));
			IPerson karol=entities.Where(item => item.Id==(EntityId)"http://magi/people/Karol").FirstOrDefault();
			Assert.That(karol,Is.Not.Null);
			Assert.That(karol,Is.InstanceOf<IEntity>());
			Assert.That(karol.FamilyName,Is.EqualTo("Szczepański"));
		}

		[Test]
		[Repeat(5)]
		public void Selecting_entities_by_providing_nested_predicate_value_condition_test()
		{
			IEntity entity=(from resources in _entityContext.AsQueryable<IEntity>()
							where ((IList<IEntity>)((OntologyAccessor)resources["foaf"])["knows"]).Any(item => item.Id==(EntityId)"http://magi/people/Tomasz")
						   select resources).FirstOrDefault();
			Assert.That(entity,Is.Not.Null);
			Assert.That(entity,Is.InstanceOf<Entity>());
			Assert.That(entity.AsDynamic().foaf.givenName,Is.EqualTo("Karol"));
			Assert.That(entity.AsDynamic().foaf.familyName,Is.EqualTo("Szczepański"));
		}
	}
}