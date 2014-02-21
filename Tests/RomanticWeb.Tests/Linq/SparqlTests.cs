using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RomanticWeb.Converters;
using RomanticWeb.DotNetRDF;
using RomanticWeb.Entities;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Fluent;
using RomanticWeb.Mapping.Model;
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

        public interface IPerson:IEntity
        {
            string FirstName { get; }
         
            string Surname { get; }

            List<IPerson> Knows { get; }
        }

        [SetUp]
        public void Setup()
        {
            _store=new TripleStore();
            _store.LoadTestFile("TriplesWithLiteralSubjects.trig"); 
            
            _factory=new Mock<IEntityContextFactory>();
            _factory.Setup(cf => cf.Converters).Returns(new ConverterCatalog());

            _baseUriSelectionPolicy=new Mock<IBaseUriSelectionPolicy>();
            _baseUriSelectionPolicy.Setup(policy => policy.SelectBaseUri(It.IsAny<EntityId>())).Returns(new Uri("http://magi/"));

            var ontologyProvider=new CompoundOntologyProvider(new DefaultOntologiesProvider());
            _mappingsRepository = new TestMappingsRepository(new TestPersonMap(), new TestTypedEntityMap());
            var mappingContext = new MappingContext(ontologyProvider, new DefaultGraphSelector());
            _mappingsRepository.RebuildMappings(mappingContext);
            _entityContext=new EntityContext(_factory.Object,_mappingsRepository,mappingContext,new EntityStore(),new TripleStoreAdapter(_store),_baseUriSelectionPolicy.Object);
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
            IList<IEntity> entities = (from resources in _entityContext.AsQueryable<IEntity>()
                                     where resources is IPerson
                                     select resources).ToList();
            Assert.That(entities.Count,Is.EqualTo(2));
            IEntity tomasz=entities.Where(item => item.Id==(EntityId)"http://magi/people/Tomasz").FirstOrDefault();
            Assert.That(tomasz,Is.Not.Null);
            Assert.That(tomasz,Is.InstanceOf<IPerson>());
            Assert.That(tomasz.AsDynamic().foaf.first_givenName,Is.EqualTo("Tomasz"));
            Assert.That(tomasz.AsDynamic().foaf.first_familyName,Is.EqualTo("Pluskiewicz"));
            IEntity gniewoslaw=entities.Where(item => item.Id==(EntityId)"http://magi/people/Gniewoslaw").FirstOrDefault();
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

        private class TestPersonMap : EntityMap<IPerson>
        {
            public TestPersonMap()
            {
                Class.Is("foaf", "Person");
                Property(p => p.Knows).Term.Is("foaf", "knows").NamedGraph.SelectedBy<TestGraphSelector>();
                Property(p => p.FirstName).Term.Is("foaf", "givenName").NamedGraph.SelectedBy<TestGraphSelector>();
                Property(p => p.Surname).Term.Is("foaf", "familyName").NamedGraph.SelectedBy<TestGraphSelector>();
            }
        }

        private class TestTypedEntityMap : EntityMap<ITypedEntity>
        {
            public TestTypedEntityMap()
            {
                Class.Is(Vocabularies.Rdfs.Class);
                Collection(p => p.Types).Term.Is(Vocabularies.Rdf.type).NamedGraph.SelectedBy<TestGraphSelector>();
            }
        }
    }
}