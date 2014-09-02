using System;
using System.Collections.Generic;
using System.Linq;
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
    public class SparqlResultModifiersTests
    {
        private IEntityContext _entityContext;

        public interface IPerson : IEntity
        {
            string FirstName { get; }

            string Surname { get; }

            List<IPerson> Knows { get; }
        }

        [SetUp]
        public void Setup()
        {
            IServiceContainer container = new ServiceContainer();
            IEntityContextFactory factory = new EntityContextFactory(container)
               .WithDefaultOntologies()
               .WithMetaGraphUri(new Uri("http://app.magi/graphs"))
               .WithDependencies<Dependencies>();

            _entityContext = factory.CreateContext();
        }

        [Test]
        [TestCase(false, new string[] { "Dominik", "Gniewosław", "Karol", "Mirosław", "Monika", "Przemysław", "Tomasz" })]
        [TestCase(true, new string[] { "Tomasz", "Przemysław", "Monika", "Mirosław", "Karol", "Gniewosław", "Dominik" })]
        public void Selecting_entities_ordered_by_properties(bool descending, string[] expected)
        {
            IList<IPerson> entities = (!descending ? _entityContext.AsQueryable<IPerson>().OrderBy(person => person.FirstName).ThenBy(person => person.Surname) :
                _entityContext.AsQueryable<IPerson>().OrderByDescending(person => person.FirstName).ThenByDescending(person => person.Surname)).ToList();
            for (int index = 0; index < expected.Length; index++)
            {
                Assert.That(entities[index].FirstName, Is.EqualTo(expected[index]));
            }
        }

        [Test]
        [TestCase(false, 2, 2, new string[] { "Dominik", "Gniewosław", "Karol", "Mirosław", "Monika", "Przemysław", "Tomasz" })]
        [TestCase(true, 3, 3, new string[] { "Tomasz", "Przemysław", "Monika", "Mirosław", "Karol", "Gniewosław", "Dominik" })]
        public void Selecting_subset_of_entities_with_order_kept(bool descending, int offset, int limit, string[] possibilities)
        {
            IList<IPerson> entities = (!descending ? _entityContext.AsQueryable<IPerson>().OrderBy(person => person.FirstName) :
                _entityContext.AsQueryable<IPerson>().OrderByDescending(person => person.FirstName)).Skip(offset).Take(limit).ToList();
            Assert.That(entities.Count, Is.EqualTo(limit));
            for (int index = 0; index < limit; index++)
            {
                Assert.That(entities[index].FirstName, Is.EqualTo(possibilities[index + offset]));
            }
        }

        private class TestPersonMap : TestEntityMapping<IPerson>
        {
            public TestPersonMap()
            {
                Class(Vocabularies.Foaf.Person);
                Collection("Knows", Vocabularies.Foaf.knows, typeof(List<IPerson>), new AsEntityConverter<IPerson>());
                Property("FirstName", Vocabularies.Foaf.givenName, typeof(string), new StringConverter());
                Property("Surname", Vocabularies.Foaf.familyName, typeof(string), new StringConverter());
            }
        }

        private class TestTypedEntityMap : TestEntityMapping<ITypedEntity>
        {
            public TestTypedEntityMap()
            {
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
                _baseUriSelectionPolicy.Setup(policy => policy.SelectBaseUri(It.IsAny<EntityId>()))
                                       .Returns(new Uri("http://magi/"));
            }

            public void Compose(IServiceRegistry serviceRegistry)
            {
                serviceRegistry.RegisterInstance<IRdfTypeCache>(_typeCache);
                serviceRegistry.RegisterInstance(_baseUriSelectionPolicy.Object);

                var repository = new TestMappingsRepository(new TestPersonMap(), new TestTypedEntityMap());
                serviceRegistry.RegisterInstance<IMappingsRepository>(repository);

                serviceRegistry.Register<INamedGraphSelector, TestGraphSelector>();

                var store = new TripleStore();
                store.LoadTestFile("SuperTripleOperations.trig");
                serviceRegistry.RegisterInstance<ITripleStore>(store);
                serviceRegistry.Register<IEntitySource, TripleStoreAdapter>();
            }
        }
    }
}