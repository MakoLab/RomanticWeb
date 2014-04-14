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
    public class SparqlResultModifiersTests
    {
        private IEntityContext _entityContext;
        private TripleStore _store;
        private TestMappingsRepository _mappingsRepository;
        private Mock<IEntityContextFactory> _factory;
        private Mock<IBaseUriSelectionPolicy> _baseUriSelectionPolicy;
        private TestCache _typeCache;

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
            _store.LoadTestFile("SuperTripleOperations.trig"); 
            
            _factory=new Mock<IEntityContextFactory>();
            _baseUriSelectionPolicy=new Mock<IBaseUriSelectionPolicy>();
            _baseUriSelectionPolicy.Setup(policy => policy.SelectBaseUri(It.IsAny<EntityId>())).Returns(new Uri("http://magi/"));
            
            var ontologyProvider=new CompoundOntologyProvider(new DefaultOntologiesProvider());
            _mappingsRepository=new TestMappingsRepository(new TestPersonMap(),new TestTypedEntityMap());
            var mappingContext=new MappingContext(ontologyProvider,EntityContextFactory.CreateDefaultConventions());
            _typeCache=new TestCache();
            _entityContext=new EntityContext(
                _factory.Object,
                _mappingsRepository,
                mappingContext,
                new EntityStore(),
                new TripleStoreAdapter(_store) { MetaGraphUri=new Uri("http://app.magi/graphs") },
                _baseUriSelectionPolicy.Object,
                new TestGraphSelector(),
                _typeCache);
        }

        [Test]
        [TestCase(false,new string[] { "Dominik","Gniewosław","Karol","Mirosław","Monika","Przemysław","Tomasz" })]
        [TestCase(true,new string[] { "Tomasz","Przemysław","Monika","Mirosław","Karol","Gniewosław","Dominik" })]
        public void Selecting_entities_ordered_by_properties(bool descending,string[] expected)
        {
            IList<IPerson> entities=(!descending?_entityContext.AsQueryable<IPerson>().OrderBy(person => person.FirstName).ThenBy(person => person.Surname):
                _entityContext.AsQueryable<IPerson>().OrderByDescending(person => person.FirstName).ThenByDescending(person => person.Surname)).ToList();
            for (int index=0; index<expected.Length; index++)
            {
                Assert.That(entities[index].FirstName,Is.EqualTo(expected[index]));
            }
        }

        [Test]
        [TestCase(false,2,2,new string[] { "Dominik","Gniewosław","Karol","Mirosław","Monika","Przemysław","Tomasz" })]
        [TestCase(true,3,3,new string[] { "Tomasz","Przemysław","Monika","Mirosław","Karol","Gniewosław","Dominik" })]
        public void Selecting_subset_of_entities_with_order_kept(bool descending,int offset,int limit,string[] possibilities)
        {
            IList<IPerson> entities=(!descending?_entityContext.AsQueryable<IPerson>().OrderBy(person => person.FirstName):
                _entityContext.AsQueryable<IPerson>().OrderByDescending(person => person.FirstName)).Skip(offset).Take(limit).ToList();
            Assert.That(entities.Count,Is.EqualTo(limit));
            for (int index=0; index<limit; index++)
            {
                Assert.That(entities[index].FirstName,Is.EqualTo(possibilities[index+offset]));
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