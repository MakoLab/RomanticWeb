using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using RomanticWeb.Entities;
using RomanticWeb.Mapping;
using RomanticWeb.TestEntities;
using RomanticWeb.Tests.Stubs;

namespace RomanticWeb.Tests.IntegrationTests
{
    [TestFixture]
    public abstract class MappingTests : IntegrationTestsBase
    {
        protected IPerson Entity
        {
            get { return EntityFactory.Create<IPerson>(new EntityId("http://magi/people/Tomasz")); }
        }

        private new TestMappingsRepository Mappings
        {
            get { return (TestMappingsRepository)base.Mappings; }
        }

		[Test]
		public void Property_should_be_mapped_to_default_graph()
		{
			// given
			Mappings.Add(new DefaultGraphPersonMapping());
			LoadTestFile("TriplesWithLiteralSubjects.trig");

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
			LoadTestFile("LooseCollections.trig");

			// when
			var interests = Entity.Interests;

			// then
			Assert.That(interests, Has.Count.EqualTo(5));
			interests.Should().Contain(new object[] { "RDF", "Semantic Web", "C#", "Big data", "Web 3.0" });
        }

        [Test]
        public void Mapping_rdflist_of_entites_should_be_possible()
        {
            // given
            Mappings.Add(new DefaultGraphPersonMapping());
            LoadTestFile("RdfLists.meta.ttl", new Uri("http://app.magi/graphs"));
            LoadTestFile("RdfLists.math.ttl", new Uri("urn:test:array"));
            LoadTestFile("RdfLists.tomasz.ttl", new Uri("http://data.magi/people/Tomasz"));

            // when
            var friends = Entity.Friends;

            // then
            Assert.That(friends, Has.Count.EqualTo(5));
            friends.Should()
                   .ContainInOrder(
                       new Entity(new EntityId("http://magi/people/Karol")),
                       new Entity(new EntityId("http://magi/people/Gniewko")),
                       new Entity(new EntityId("http://magi/people/Monika")),
                       new Entity(new EntityId("http://magi/people/Dominik")),
                       new Entity(new EntityId("http://magi/people/Przemek")));
        }

        [Test]
        public void Mapping_loose_collection_of_entites_should_be_possible()
        {
            // given
            Mappings.Add(new DefaultGraphPersonMapping());
            LoadTestFile("LooseCollections.trig");

            // when
            var friends = Entity.Friends;

            // then
            Assert.That(friends, Has.Count.EqualTo(5));
            friends.Should().Contain(new[]
                                       {
                                           new Entity(new EntityId("http://magi/people/Karol")),
                                           new Entity(new EntityId("http://magi/people/Gniewko")),
                                           new Entity(new EntityId("http://magi/people/Monika")),
                                           new Entity(new EntityId("http://magi/people/Dominik")),
                                           new Entity(new EntityId("http://magi/people/Przemek"))
                                       });
        }

        [Test]
        public void Mapping_loose_collection_of_entites_should_be_possible_if_only_one_element_is_present()
        {
            // given
            Mappings.Add(new DefaultGraphPersonMapping());
            LoadTestFile("AssociatedInstances.trig");

            // when
            IList<IPerson> friends = Entity.Friends;

            // then
            Assert.That(friends, Has.Count.EqualTo(1));
            friends.Should().Contain(new[] { new Entity(new EntityId("http://magi/people/Karol")) });
        }

        [Test]
        public void Mapping_loose_collection_of_entites_should_be_possible_if_only_are_no_elements_present()
        {
            // given
            Mappings.Add(new DefaultGraphPersonMapping());
            LoadTestFile("AssociatedInstances.trig");

            // when
            var friends = Entity.Interests;

            // then
            Assert.That(friends, Has.Count.EqualTo(0));
        }

        protected override IMappingsRepository SetupMappings()
        {
            return new TestMappingsRepository(new TestOntologyProvider());
        }
    }
}
