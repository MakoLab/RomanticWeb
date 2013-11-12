using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RomanticWeb.Entities;
using RomanticWeb.Mapping;
using RomanticWeb.Model;
using RomanticWeb.TestEntities;
using RomanticWeb.Tests.Stubs;

namespace RomanticWeb.Tests.IntegrationTests
{
    [TestFixture]
    public abstract class MappingTests : IntegrationTestsBase
    {
        protected IPerson Entity
        {
            get { return EntityContext.Load<IPerson>(new EntityId("http://magi/people/Tomasz")); }
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
        public void Mapped_collection_of_entites_should_allow_accessing_members()
        {
            // given
            Mappings.Add(new DefaultGraphPersonMapping());
            LoadTestFile("LooseCollections.trig");

            // when
            var friends = Entity.Friends;

            // then
            Assert.That(friends, Has.Count.EqualTo(5));
            friends.Select(friend => friend.FirstName)
                   .Should().Contain(new[] { "Karol","Gniewko","Monika","Dominik","Przemek" });
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

        [Test]
        public void Mapping_blank_node_rdf_collection_of_entities_should_be_possible()
        {
            // given
            Mappings.Add(new DefaultGraphPersonMapping());
            LoadTestFile("RdfLists.meta.ttl", new Uri("http://app.magi/graphs"));
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
        public void Mapping_URI_node_rdf_collection_of_entities_should_be_possible()
        {
            // given
            Mappings.Add(new DefaultGraphPersonMapping());
            LoadTestFile("RdfLists.meta.ttl", new Uri("http://app.magi/graphs"));
            LoadTestFile("RdfLists.tomasz.ttl", new Uri("http://data.magi/people/Tomasz"));

            // when
            var friends = Entity.NickNames;

            // then
            Assert.That(friends, Has.Count.EqualTo(2));
            friends.Should().Contain("Tomek", "Tomasz");
        }

        [Test]
        public void Should_throw_if_a_property_returns_an_rdf_collection()
        {
            Mappings.Add(new DefaultGraphPersonMapping());
            LoadTestFile("Collections.trig");

            // then
            Assert.Throws<CardinalityException>(() => { var hp = Entity.Homepage; });
        }

        [Test]
        public void Should_throw_if_a_property_returns_multiple_values()
        {
            Mappings.Add(new DefaultGraphPersonMapping());
            LoadTestFile("Collections.trig");

            // then
            var cardinalityException=Assert.Throws<CardinalityException>(() => { var hp=Entity.FirstName; });
            Assert.That(cardinalityException.ExpectedCardinality, Is.EqualTo(1));
            Assert.That(cardinalityException.ActualCardinality, Is.EqualTo(2));
        }

        [Test]
        public void Should_throw_if_a_collection_is_backed_by_a_list_and_a_direct_relation()
        {
            Mappings.Add(new DefaultGraphPersonMapping());
            LoadTestFile("MixedCollections.trig");

            // then
            var cardinalityException=Assert.Throws<CardinalityException>(() => { var hp=Entity.Homepage; });
            Assert.That(cardinalityException.ExpectedCardinality, Is.EqualTo(1));
            Assert.That(cardinalityException.ActualCardinality, Is.EqualTo(3));
        }

        [Test]
        public void Should_throw_if_a_collection_is_backed_by_two_lists()
        {
            Mappings.Add(new DefaultGraphPersonMapping());
            LoadTestFile("MixedCollections.trig");

            // then
            Assert.Throws<CardinalityException>(() => { var hp = Entity.Homepage; });
        }

        [Test]
        public void Setting_property_should_be_possible_in_default_graph()
        {
            // given
            Mappings.Add(new DefaultGraphPersonMapping());
            LoadTestFile("TriplesWithLiteralSubjects.trig");

            // when
            Entity.FirstName="Michał";

            // then
            Assert.That(Entity.FirstName, Is.EqualTo("Michał"));
            Assert.That(EntityStore.Quads, Has.Count.EqualTo(3));
        }

        [Test]
        public void Setting_property_should_be_possible_in_named_graph()
        {
            // given
            Mappings.Add(new NamedGraphsPersonMapping());
            LoadTestFile("TriplesInNamedGraphs.trig");

            // when
            Entity.FirstName = "Dominik";
            Entity.LastName = "Kuziński";

            // then
            Assert.That(Entity.FirstName, Is.EqualTo("Dominik"));
            Assert.That(Entity.LastName, Is.EqualTo("Kuziński"));
            Assert.That(EntityStore.Quads, Has.Count.EqualTo(4), "Actual triples were: {0}", SerializeStore());
            var quads=EntityStore.Quads.Where(q => q.Graph==Node.ForUri(new Uri("personal://magi/people/Tomasz")));
            Assert.That(quads.ToList(), Has.Count.EqualTo(2), "Actual triples were: {0}", SerializeStore());
        }

        [Test]
        public void Should_allow_chaining_typed_entities()
        {
            // given
            Mappings.Add(new NamedGraphsPersonMapping());
            LoadTestFile("AssociatedInstances.trig");

            // when
            var karol=Entity.Friend;

            // then
            Assert.That(karol, Is.InstanceOf<IPerson>());
            Assert.That(karol.FirstName, Is.EqualTo("Karol"));
        }

        protected override IMappingsRepository SetupMappings()
        {
            return new TestMappingsRepository();
        }

        private string SerializeStore()
        {
            return String.Join(Environment.NewLine,EntityStore.Quads);
        }
    }
}
