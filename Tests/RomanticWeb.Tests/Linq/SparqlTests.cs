using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RomanticWeb.Converters;
using RomanticWeb.DotNetRDF;
using RomanticWeb.Entities;
using RomanticWeb.LightInject;
using RomanticWeb.Mapping;
using RomanticWeb.NamedGraphs;
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
        private TestCache _typeCache;

        public interface IAddress : IEntity
        {
            string City { get; set; }

            string Street { get; set; }
        }

        public interface IPerson : IEntity
        {
            string FirstName { get; }

            string Surname { get; }

            List<IPerson> Knows { get; }

            IAddress Address { get; }

            DateTime CreatedOn { get; }
        }

        [SetUp]
        public void Setup()
        {
            _store = new TripleStore();
            _store.LoadTestFile("TriplesWithLiteralSubjects.trig");

            IServiceContainer container = new ServiceContainer();
            IEntityContextFactory factory = new EntityContextFactory(container)
                .WithDefaultOntologies()
                .WithEntitySource(() => new TripleStoreAdapter(_store))
                .WithMetaGraphUri(new Uri("http://app.magi/graphs"))
                .WithDependenciesInternal<Dependencies>();

            _typeCache = (TestCache)container.GetInstance<IRdfTypeCache>();

            _entityContext = factory.CreateContext();
        }

        [Test]
        [Repeat(5)]
        public void Selecting_entities_by_providing_single_literal_predicate_value_condition_from_pointed_ontology_test()
        {
            IList<IPerson> entities = (from resources in _entityContext.AsQueryable<IPerson>()
                                       where resources.FirstName == "Tomasz"
                                       select resources).ToList();
            Assert.That(entities.Count, Is.EqualTo(1));
            Assert.That(entities[0], Is.Not.Null);
            Assert.That(entities[0], Is.InstanceOf<IPerson>());
            Assert.That(entities[0].FirstName, Is.EqualTo("Tomasz"));
            Assert.That(entities[0].Surname, Is.EqualTo("Pluskiewicz"));
        }

        [Test]
        [Repeat(5)]
        public void Selecting_entities_by_providing_single_literal_predicate_value_condition_test()
        {
            IList<IPerson> entities = (from resources in _entityContext.AsQueryable<IPerson>()
                                       where resources.FirstName == "Tomasz"
                                       select resources).ToList();
            Assert.That(entities.Count, Is.EqualTo(1));
            Assert.That(entities[0], Is.Not.Null);
            Assert.That(entities[0], Is.InstanceOf<IEntity>());
            Assert.That(entities[0].FirstName, Is.EqualTo("Tomasz"));
            Assert.That(entities[0].Surname, Is.EqualTo("Pluskiewicz"));
        }

        [Test]
        [Repeat(5)]
        public void Selecting_entities_by_providing_subject_identifier_condition_test()
        {
            IPerson entity = (from resources in _entityContext.AsQueryable<IPerson>()
                              where resources.Id == (EntityId)"http://magi/people/Tomasz"
                              select resources).FirstOrDefault();
            Assert.That(entity, Is.Not.Null);
            Assert.That(entity, Is.InstanceOf<IEntity>());
            Assert.That(entity.FirstName, Is.EqualTo("Tomasz"));
            Assert.That(entity.Surname, Is.EqualTo("Pluskiewicz"));
        }

        [Test]
        [Repeat(5)]
        public void Selecting_entities_by_providing_entity_mapped_type_condition_test()
        {
            _typeCache.Setup<IEntity, IPerson>();
            IList<IEntity> entities = (from resources in _entityContext.AsQueryable<IEntity>()
                                       where resources is IPerson
                                       select resources).ToList();
            Assert.That(entities.Count, Is.EqualTo(3));
            IEntity tomasz = entities.FirstOrDefault(item => item.Id == (EntityId)"http://magi/people/Tomasz");
            Assert.That(tomasz, Is.Not.Null);
            Assert.That(tomasz, Is.InstanceOf<IPerson>());
            Assert.That(tomasz.AsDynamic().foaf.first_givenName, Is.EqualTo("Tomasz"));
            Assert.That(tomasz.AsDynamic().foaf.first_familyName, Is.EqualTo("Pluskiewicz"));
            IEntity gniewoslaw = entities.FirstOrDefault(item => item.Id == (EntityId)"http://magi/people/Gniewoslaw");
            Assert.That(gniewoslaw, Is.Not.Null);
            Assert.That(gniewoslaw, Is.InstanceOf<IPerson>());
            Assert.That(gniewoslaw.AsDynamic().foaf.first_givenName, Is.EqualTo("Gniewosław"));
            Assert.That(gniewoslaw.AsDynamic().foaf.first_familyName, Is.EqualTo("Rzepka"));
            IEntity dominik = entities.FirstOrDefault(item => item.Id == (EntityId)"http://magi/people/Dominik");
            Assert.That(dominik, Is.Not.Null);
            Assert.That(dominik, Is.InstanceOf<IPerson>());
            Assert.That(dominik.AsDynamic().foaf.first_givenName, Is.EqualTo("Dominik"));
            Assert.That((string)dominik.AsDynamic().foaf.firstOrDefault_familyName, Is.Null);
        }

        [Test]
        [Repeat(5)]
        public void Selecting_specific_type_entities_test()
        {
            IList<IPerson> entities = (from resources in _entityContext.AsQueryable<IPerson>()
                                       select resources).ToList();
            Assert.That(entities.Count, Is.EqualTo(3));
            IPerson tomasz = entities.Where(item => item.Id == (EntityId)"http://magi/people/Tomasz").FirstOrDefault();
            Assert.That(tomasz, Is.Not.Null);
            Assert.That(tomasz, Is.InstanceOf<IEntity>());
            Assert.That(tomasz.Surname, Is.EqualTo("Pluskiewicz"));
            IPerson gniewoslaw = entities.Where(item => item.Id == (EntityId)"http://magi/people/Gniewoslaw").FirstOrDefault();
            Assert.That(gniewoslaw, Is.Not.Null);
            Assert.That(gniewoslaw, Is.InstanceOf<IEntity>());
            Assert.That(gniewoslaw.Surname, Is.EqualTo("Rzepka"));
            IPerson dominik = entities.Where(item => item.Id == (EntityId)"http://magi/people/Dominik").FirstOrDefault();
            Assert.That(dominik, Is.Not.Null);
            Assert.That(dominik, Is.InstanceOf<IEntity>());
            Assert.That(dominik.Surname, Is.Null);
        }

        [Test]
        [TestCase("", 2)]
        [TestCase("Pluskiewicz", 1)]
        public void Selecting_filtered_entities(string searchString, int expectedCount)
        {
            IList<IPerson> entities = (from resources in _entityContext.AsQueryable<IPerson>()
                                       where resources.Surname.Contains(searchString)
                                       select resources).ToList();
            Assert.That(entities.Count, Is.EqualTo(expectedCount));
        }

        [Test]
        [Repeat(5)]
        public void Selecting_entities_by_providing_nested_predicate_value_condition_test()
        {
            IPerson entity = (from resources in _entityContext.AsQueryable<IPerson>()
                              where resources.Knows.Any(item => item.Id == (EntityId)"http://magi/people/Tomasz")
                              select resources).FirstOrDefault();
            Assert.That(entity, Is.Not.Null);
            Assert.That(entity, Is.InstanceOf<IPerson>());
            Assert.That(entity.FirstName, Is.EqualTo("Gniewosław"));
            Assert.That(entity.Surname, Is.EqualTo("Rzepka"));
        }

        [Test]
        public void Selecting_entitys_literal_property()
        {
            string firstName = (from resources in _entityContext.AsQueryable<IPerson>()
                                where resources.Id == (EntityId)"http://magi/people/Gniewoslaw"
                                select resources.FirstName).FirstOrDefault();
            Assert.That(firstName, Is.EqualTo("Gniewosław"));
        }

        [Test]
        public void Selecting_entitys_blank_node_IEntity_property()
        {
            IAddress address = (from resources in _entityContext.AsQueryable<IPerson>()
                                where resources.Id == (EntityId)"http://magi/people/Gniewoslaw"
                                select resources.Address).FirstOrDefault();
            Assert.That(address, Is.Not.Null);
            Assert.That(address.City, Is.EqualTo("Łódź"));
            Assert.That(address.Street, Is.EqualTo("Rzgowska 30"));
        }

        [Test]
        public void Selecting_entitys_IEntity_property()
        {
            IAddress address = (from resources in _entityContext.AsQueryable<IPerson>()
                                where resources.Id == (EntityId)"http://magi/people/Tomasz"
                                select resources.Address).FirstOrDefault();
            Assert.That(address, Is.Not.Null);
            Assert.That(address.City, Is.EqualTo("Łódź"));
            Assert.That(address.Street, Is.EqualTo("Demokratyczna 46"));
        }

        [Test]
        public void Selecting_generic_entities_by_relative_child_EntityId()
        {
            // given
            var relativeId = new EntityId("/people/Tomasz");

            // when
            var tomasz = (from person in _entityContext.AsQueryable<IPerson>()
                          where person.Id == relativeId
                          select person).SingleOrDefault();

            // then
            tomasz.Should().NotBeNull();
        }

        [Test]
        public void Materializing_query_twice()
        {
            IEnumerable<IPerson> query = _entityContext.AsQueryable<IPerson>().Where(person => person.FirstName.Length > 0);
            foreach (IPerson person in query)
            {
                Assert.That(person, Is.Not.Null);
            }

            foreach (IPerson person in query)
            {
                Assert.That(person.FirstName, Is.Not.Null);
                Assert.That(person.FirstName.Length, Is.GreaterThan(0));
            }
        }

        [Test]
        public void Selecting_by_dates()
        {
            IList<IPerson> persons = (from person in _entityContext.AsQueryable<IPerson>()
                                      where person.CreatedOn <= DateTime.Now
                                      select person).ToList();
            Assert.That(persons.Count, Is.EqualTo(2));
        }

        [Test]
        public void Selecting_with_multiple_filter_transformatons()
        {
            IList<IPerson> persons = (from person in _entityContext.AsQueryable<IPerson>()
                                      where person.Surname.ToLower().Contains("k")
                                      select person).ToList();
            Assert.That(persons.Count, Is.EqualTo(2));
        }

        [Test]
        public void Select_EntityIds_property()
        {
            IList<Uri> uris = (from person in _entityContext.AsQueryable<IPerson>()
                               select person.Id.Uri).ToList();
            Assert.That(uris.Count, Is.EqualTo(3));
        }

        [Test]
        public void Select_where_property_is_not_null()
        {
            IList<IPerson> entities = (from resources in _entityContext.AsQueryable<IPerson>()
                                       where resources.Address != null
                                       select resources).ToList();
            Assert.That(entities.Count, Is.EqualTo(2));
        }

        [Test]
        public void Select_where_property_is_null()
        {
            IList<IPerson> entities = (from resources in _entityContext.AsQueryable<IPerson>()
                                       where resources.Address == null
                                       select resources).ToList();
            Assert.That(entities.Count, Is.EqualTo(1));
        }

        [Test]
        public void Select_where_literal_property_is_not_null()
        {
            IList<IPerson> entities = (from resources in _entityContext.AsQueryable<IPerson>()
                                       where resources.Surname != null
                                       select resources).ToList();
            Assert.That(entities.Count, Is.EqualTo(2));
        }

        [Test]
        public void Select_where_literal_property_is_null()
        {
            IList<IPerson> entities = (from resources in _entityContext.AsQueryable<IPerson>()
                                       where resources.Surname == null
                                       select resources).ToList();
            Assert.That(entities.Count, Is.EqualTo(1));
        }

        [Test]
        public void Select_entities_filtered_with_binary_operator()
        {
            Uri searchedUri = new Uri("http://magi/addresses/Address");
            IList<IPerson> entities = (from resources in _entityContext.AsQueryable<IPerson>()
                                       where (resources.Address != null) && (resources.Address.Id.Uri == searchedUri)
                                       select resources).ToList();
            Assert.That(entities.Count, Is.EqualTo(1));
        }

        [Test]
        public void Select_entities_with_multiple_where_statemens()
        {
            Uri searchedUri = new Uri("http://magi/addresses/Address");
            IList<IPerson> entities = (from resources in _entityContext.AsQueryable<IPerson>()
                                       where (resources.Address != null)
                                       where (resources.Address.Id.Uri == searchedUri)
                                       select resources).ToList();
            Assert.That(entities.Count, Is.EqualTo(1));
        }

        [Test]
        public void Select_where_subquery_matches()
        {
            Uri searched = new Uri("http://magi/people/Tomasz");
            IEnumerable<IPerson> persons = from person in _entityContext.AsQueryable<IPerson>()
                                           from friend in person.Knows
                                           where friend.Id.Uri == searched
                                           select friend;
            Assert.That(persons.Count(), Is.EqualTo(1));
        }

        [Test]
        public void Select_elements_of_collection()
        {
            IEnumerable<IPerson> persons = (from person in _entityContext.AsQueryable<IPerson>()
                                            from friend in person.Knows
                                            where person.Id == "http://magi/people/Gniewoslaw"
                                            where friend.FirstName == "Tomasz"
                                            select friend);

            Assert.That(persons.Single().Id, Is.EqualTo(new EntityId("http://magi/people/Tomasz")));
        }

        [Test]
        public void Select_filter_by_collection_elements()
        {
            IEnumerable<IPerson> persons = (from person in _entityContext.AsQueryable<IPerson>()
                                            from friend in person.Knows
                                            where friend.FirstName == "Tomasz"
                                            select person).ToList();
            
            Assert.That(persons.Count(), Is.EqualTo(1));
        }

        private class TestPersonMap : TestEntityMapping<IPerson>
        {
            public TestPersonMap()
            {
                Class(Vocabularies.Foaf.Person);
                Collection("Knows", Vocabularies.Foaf.knows, typeof(List<IPerson>), new AsEntityConverter<IPerson>());
                Property("FirstName", Vocabularies.Foaf.givenName, typeof(string), new StringConverter());
                Property("Surname", Vocabularies.Foaf.familyName, typeof(string), new StringConverter());
                Property("Address", new Uri("http://schema.org/address"), typeof(IAddress), new AsEntityConverter<IAddress>());
                Property("CreatedOn", new Uri("http://purl.org/dc/elements/1.1/date"), typeof(DateTime), new DateTimeConverter());
            }
        }

        private class TestAdressMap : TestEntityMapping<IAddress>
        {
            public TestAdressMap()
            {
                Class(new Uri("http://schema.org/PostalAddress"));
                Property("City", new Uri("http://schema.org/addressLocality"), typeof(string), new StringConverter());
                Property("Street", new Uri("http://schema.org/streetAddress"), typeof(string), new StringConverter());
            }
        }

        private class TestTypedEntityMap : TestEntityMapping<ITypedEntity>
        {
            public TestTypedEntityMap()
            {
                Class(Vocabularies.Rdfs.Class);
                Collection("Types", Vocabularies.Rdf.type, typeof(ICollection<EntityId>), new EntityIdConverter());
            }
        }

        private class Dependencies : ICompositionRoot
        {
            private readonly TestCache _typeCache;
            private readonly Mock<IBaseUriSelectionPolicy> _baseUriSelectionPolicy;

            public Dependencies()
            {
                _typeCache = new TestCache();
                _baseUriSelectionPolicy = new Mock<IBaseUriSelectionPolicy>();
                _baseUriSelectionPolicy.Setup(policy => policy.SelectBaseUri(It.IsAny<EntityId>())).Returns(new Uri("http://magi/"));
            }

            public void Compose(IServiceRegistry serviceRegistry)
            {
                serviceRegistry.RegisterInstance<IRdfTypeCache>(_typeCache);
                serviceRegistry.RegisterInstance(_baseUriSelectionPolicy.Object);
                serviceRegistry.Register<INamedGraphSelector, TestGraphSelector>();

                var repository = new TestMappingsRepository(new TestPersonMap(), new TestTypedEntityMap(), new TestAdressMap());
                serviceRegistry.RegisterInstance<IMappingsRepository>(repository);

                serviceRegistry.Register<INodeConverter, StringConverter>("String converter");
            }
        }
    }
}