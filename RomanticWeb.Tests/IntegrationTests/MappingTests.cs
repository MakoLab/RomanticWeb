using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RomanticWeb.Mapping;
using RomanticWeb.TestEntities;

namespace RomanticWeb.Tests.IntegrationTests
{
    [TestFixture]
    public class MappingTests : InMemoryTripleStoreTestsBase
    {
        private new TestMappingsRepository Mappings
        {
            get { return (TestMappingsRepository)base.Mappings; }
        }

        protected IPerson Entity
        {
            get { return EntityFactory.Create<IPerson>(new UriId("http://magi/people/Tomasz")); }
        }

        protected override IMappingsRepository SetupMappings()
        {
            return new TestMappingsRepository();
        }

        [Test]
        public void Property_should_be_mapped_to_default_graph()
        {
            // given
            Mappings.Add(new DefaultGraphPersonMapping());
            LoadTestFile("TriplesWithLiteralSubjects.ttl");

            // when
            string firstName = Entity.FirstName;

            // then
            Assert.That(firstName, Is.EqualTo("Tomasz"));
        }

        [Test]
        public void Mapping_property_to_specific_graph_should_be_possible()
        {
            // given
            Mappings.Add(new NamedGraphsPersonMapping());
            LoadTestFile("TriplesInNamedGraphs.trig");

            // when
            string firstName = Entity.FirstName;
            string lastName = Entity.LastName;

            // then
            Assert.That(firstName, Is.EqualTo("Tomasz"));
            Assert.That(lastName, Is.EqualTo("Pluskiewicz"));
        }

        [Test]
        public void Mapping_simple_collections_should_be_possible()
        {
            // given
            Mappings.Add(new DefaultGraphPersonMapping());
            LoadTestFile("LooseCollections.ttl");

            // when
            var interests = Entity.Interests;

            // then
            Assert.That(interests, Has.Count.EqualTo(5));
            interests.Should().Contain(new object[] { "RDF", "Semantic Web", "C#", "Big data", "Web 3.0" });
        }
    }

    public class TestMappingsRepository : IMappingsRepository
    {
        private readonly List<EntityMap> _entityMaps;

        public TestMappingsRepository(params EntityMap[] entityMaps)
        {
            _entityMaps = entityMaps.ToList();
        }

        public IMapping MappingFor<TEntity>()
        {
            return _entityMaps.Where(map => map.EntityType == typeof(TEntity)).Cast<IMappingProvider>().First().GetMapping();
        }

        public void Add(EntityMap personMapping)
        {
            _entityMaps.Add(personMapping);
        }
    }
}
