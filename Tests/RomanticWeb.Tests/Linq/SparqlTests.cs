using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RomanticWeb.Converters;
using RomanticWeb.DotNetRDF;
using RomanticWeb.Entities;
using RomanticWeb.Mapping;
using RomanticWeb.Ontologies;
using RomanticWeb.Tests.Helpers;
using RomanticWeb.Tests.Stubs;
using VDS.RDF;

namespace RomanticWeb.Tests.Linq
{
    [TestFixture]
    public class SparqlTests
    {
        private IEntityContext _entityContext;
        private TripleStore _store;
        private TestMappingsRepository _mappingsRepository;
        private Mock<IEntityContextFactory> _factory;
        private Mock<IBaseUriSelectionPolicy> _baseUriSelectionPolicy;
        private TestCache _typeCache;

        public interface IAddress:IEntity
        {
            string City { get; set; }

            string Street { get; set; }
        }

        public interface IPerson:IEntity
        {
            string FirstName { get; }
         
            string Surname { get; }

            List<IPerson> Knows { get; }

            IAddress Address { get; }
        }

        [SetUp]
        public void Setup()
        {
            _store=new TripleStore();
            _store.LoadTestFile("TriplesWithLiteralSubjects.trig"); 
            
            _factory=new Mock<IEntityContextFactory>();
            _baseUriSelectionPolicy=new Mock<IBaseUriSelectionPolicy>();
            _baseUriSelectionPolicy.Setup(policy => policy.SelectBaseUri(It.IsAny<EntityId>())).Returns(new Uri("http://magi/"));
            
            var ontologyProvider=new CompoundOntologyProvider(new DefaultOntologiesProvider());
            _mappingsRepository=new TestMappingsRepository(new TestPersonMap(),new TestTypedEntityMap(),new TestAdressMap());
            var mappingContext=new MappingContext(ontologyProvider,EntityContextFactory.CreateDefaultConventions());
            _typeCache=new TestCache();
            var entitySource=new TripleStoreAdapter(_store) { MetaGraphUri=new Uri("http://app.magi/graphs") };
            _entityContext=new EntityContext(
                _factory.Object,
                _mappingsRepository,
                mappingContext,
                new EntityStore(),
                entitySource,
                _baseUriSelectionPolicy.Object,
                new TestGraphSelector(),
                _typeCache);
        }

        [Test]
        [Repeat(5)]
        public void Selecting_entities_by_providing_single_literal_predicate_value_condition_from_pointed_ontology_test()
        {
            IList<IPerson> entities=(from resources in _entityContext.AsQueryable<IPerson>()
                                     where resources.FirstName=="Tomasz"
                                     select resources).ToList();
            Assert.That(entities.Count,Is.EqualTo(1));
            Assert.That(entities[0],Is.Not.Null);
            Assert.That(entities[0],Is.InstanceOf<IPerson>());
            Assert.That(entities[0].FirstName,Is.EqualTo("Tomasz"));
            Assert.That(entities[0].Surname,Is.EqualTo("Pluskiewicz"));
        }

        [Test]
        [Repeat(5)]
        public void Selecting_entities_by_providing_single_literal_predicate_value_condition_test()
        {
            IList<IPerson> entities=(from resources in _entityContext.AsQueryable<IPerson>()
                                     where resources.FirstName=="Tomasz"
                                     select resources).ToList();
            Assert.That(entities.Count,Is.EqualTo(1));
            Assert.That(entities[0],Is.Not.Null);
            Assert.That(entities[0],Is.InstanceOf<IEntity>());
            Assert.That(entities[0].FirstName,Is.EqualTo("Tomasz"));
            Assert.That(entities[0].Surname,Is.EqualTo("Pluskiewicz"));
        }

        [Test]
        [Repeat(5)]
        public void Selecting_entities_by_providing_subject_identifier_condition_test()
        {
            IPerson entity=(from resources in _entityContext.AsQueryable<IPerson>()
                            where resources.Id==(EntityId)"http://magi/people/Tomasz"
                            select resources).FirstOrDefault();
            Assert.That(entity,Is.Not.Null);
            Assert.That(entity,Is.InstanceOf<IEntity>());
            Assert.That(entity.FirstName,Is.EqualTo("Tomasz"));
            Assert.That(entity.Surname,Is.EqualTo("Pluskiewicz"));
        }

        [Test]
        [Repeat(5)]
        public void Selecting_entities_by_providing_entity_mapped_type_condition_test()
        {
            _typeCache.Setup<IEntity,IPerson>();
            IList<IEntity> entities=(from resources in _entityContext.AsQueryable<IEntity>()
                                     where resources is IPerson
                                     select resources).ToList();
            Assert.That(entities.Count,Is.EqualTo(2));
            IEntity tomasz=entities.FirstOrDefault(item => item.Id==(EntityId)"http://magi/people/Tomasz");
            Assert.That(tomasz,Is.Not.Null);
            Assert.That(tomasz,Is.InstanceOf<IPerson>());
            Assert.That(tomasz.AsDynamic().foaf.first_givenName,Is.EqualTo("Tomasz"));
            Assert.That(tomasz.AsDynamic().foaf.first_familyName,Is.EqualTo("Pluskiewicz"));
            IEntity gniewoslaw=entities.FirstOrDefault(item => item.Id==(EntityId)"http://magi/people/Gniewoslaw");
            Assert.That(gniewoslaw,Is.Not.Null);
            Assert.That(gniewoslaw,Is.InstanceOf<IPerson>());
            Assert.That(gniewoslaw.AsDynamic().foaf.first_givenName,Is.EqualTo("Gniewosław"));
            Assert.That(gniewoslaw.AsDynamic().foaf.first_familyName,Is.EqualTo("Rzepka"));
        }

        [Test]
        [Repeat(5)]
        public void Selecting_specific_type_entities_test()
        {
            IList<IPerson> entities=(from resources in _entityContext.AsQueryable<IPerson>()
                                     select resources).ToList();
            Assert.That(entities.Count,Is.EqualTo(2));
            IPerson tomasz=entities.Where(item => item.Id==(EntityId)"http://magi/people/Tomasz").FirstOrDefault();
            Assert.That(tomasz,Is.Not.Null);
            Assert.That(tomasz,Is.InstanceOf<IEntity>());
            Assert.That(tomasz.Surname,Is.EqualTo("Pluskiewicz"));
            IPerson gniewoslaw=entities.Where(item => item.Id==(EntityId)"http://magi/people/Gniewoslaw").FirstOrDefault();
            Assert.That(gniewoslaw,Is.Not.Null);
            Assert.That(gniewoslaw,Is.InstanceOf<IEntity>());
            Assert.That(gniewoslaw.Surname,Is.EqualTo("Rzepka"));
        }

        [Test]
        [TestCase("",2)]
        [TestCase("Pluskiewicz",1)]
        public void Selecting_filtered_entities(string searchString,int expectedCount)
        {
            IList<IPerson> entities=(from resources in _entityContext.AsQueryable<IPerson>()
                                     where resources.Surname.Contains(searchString)
                                     select resources).ToList();
            Assert.That(entities.Count,Is.EqualTo(expectedCount));
        }

        [Test]
        [Repeat(5)]
        public void Selecting_entities_by_providing_nested_predicate_value_condition_test()
        {
            IPerson entity=(from resources in _entityContext.AsQueryable<IPerson>()
                            where resources.Knows.Any(item => item.Id==(EntityId)"http://magi/people/Tomasz")
                            select resources).FirstOrDefault();
            Assert.That(entity,Is.Not.Null);
            Assert.That(entity,Is.InstanceOf<IPerson>());
            Assert.That(entity.FirstName,Is.EqualTo("Gniewosław"));
            Assert.That(entity.Surname,Is.EqualTo("Rzepka"));
        }

        [Test]
        public void Selecting_entitys_literal_property()
        {
            string firstName=(from resources in _entityContext.AsQueryable<IPerson>()
                              where resources.Id==(EntityId)"http://magi/people/Gniewoslaw"
                              select resources.FirstName).FirstOrDefault();
            Assert.That(firstName,Is.EqualTo("Gniewosław"));
        }

        [Test]
        public void Selecting_entitys_blank_node_IEntity_property()
        {
            IAddress address=(from resources in _entityContext.AsQueryable<IPerson>()
                              where resources.Id==(EntityId)"http://magi/people/Gniewoslaw"
                              select resources.Address).FirstOrDefault();
            Assert.That(address,Is.Not.Null);
            Assert.That(address.City,Is.EqualTo("Łódź"));
            Assert.That(address.Street,Is.EqualTo("Rzgowska 30"));
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Selected_entitys_blank_node_IEntity_property_should_be_read_only()
        {
            IAddress address=(from resources in _entityContext.AsQueryable<IPerson>()
                              where resources.Id==(EntityId)"http://magi/people/Gniewoslaw"
                              select resources.Address).FirstOrDefault();
            address.Street="test";
        }

        [Test]
        public void Selecting_entitys_IEntity_property()
        {
            IAddress address=(from resources in _entityContext.AsQueryable<IPerson>()
                              where resources.Id==(EntityId)"http://magi/people/Tomasz"
                              select resources.Address).FirstOrDefault();
            Assert.That(address,Is.Not.Null);
            Assert.That(address.City,Is.EqualTo("Łódź"));
            Assert.That(address.Street,Is.EqualTo("Demokratyczna 46"));
        }

        [Test]
        public void Selecting_generic_entities_by_relative_child_EntityId()
        {
            // given
            var relativeId=new EntityId("/people/Tomasz");

            // when
            var tomasz=(from person in _entityContext.AsQueryable<IPerson>()
                        where person.Id==relativeId
                          select person).SingleOrDefault();

            // then
            tomasz.Should().NotBeNull();
        }

        [Test]
        public void Materializing_query_twice()
        {
            IEnumerable<IPerson> query=_entityContext.AsQueryable<IPerson>().Where(person => person.FirstName.Length>0);
            foreach (IPerson person in query)
            {
                Assert.That(person,Is.Not.Null);
            }

            foreach (IPerson person in query)
            {
                Assert.That(person.FirstName,Is.Not.Null);
                Assert.That(person.FirstName.Length,Is.GreaterThan(0));
            }
        }

        private class TestPersonMap:TestEntityMapping<IPerson>
        {
            public TestPersonMap()
            {
                Class(Vocabularies.Foaf.Person);
                Collection("Knows",Vocabularies.Foaf.knows,typeof(List<IPerson>),new AsEntityConverter<IPerson>());
                Property("FirstName",Vocabularies.Foaf.givenName,typeof(string),new StringConverter());
                Property("Surname",Vocabularies.Foaf.familyName,typeof(string),new StringConverter());
                Property("Address",new Uri("http://schema.org/address"),typeof(IAddress),new AsEntityConverter<IAddress>());
            }
        }

        private class TestAdressMap:TestEntityMapping<IAddress>
        {
            public TestAdressMap()
            {
                Class(new Uri("http://schema.org/PostalAddress"));
                Property("City",new Uri("http://schema.org/addressLocality"),typeof(string),new StringConverter());
                Property("Street",new Uri("http://schema.org/streetAddress"),typeof(string),new StringConverter());
            }
        }

        private class TestTypedEntityMap:TestEntityMapping<ITypedEntity>
        {
            public TestTypedEntityMap()
            {
                Class(Vocabularies.Rdfs.Class);
                Collection("Types",Vocabularies.Rdf.type,typeof(ICollection<EntityId>),new EntityIdConverter());
            }
        }
    }
}