using System;
using System.Collections;
using FluentAssertions;
using NUnit.Framework;
using RomanticWeb.Entities;

namespace RomanticWeb.Tests.IntegrationTests
{
    [TestFixture]
    public abstract class DynamicTests:IntegrationTestsBase
	{
        [Test]
        public void Creating_Entity_should_allow_accessing_existing_literal_properties()
        {
            // given
            LoadTestFile("TriplesWithLiteralSubjects.trig");

            // when
            dynamic tomasz = EntityContext.Load(new EntityId("http://magi/people/Tomasz"));

            // then
            Assert.That(tomasz.foaf.first_givenName, Is.EqualTo("Tomasz"));
            Assert.That(tomasz.foaf.first_familyName, Is.EqualTo("Pluskiewicz"));
            Assert.That(tomasz.foaf.has_nick,Is.False);
        }

        [Test]
        public void Creating_Entity_should_return_empty_collection_when_triple_doesnt_exist()
        {
            // given
            LoadTestFile("TriplesWithLiteralSubjects.trig");

            // when
            dynamic tomasz = EntityContext.Load(new EntityId("http://magi/people/Tomasz"));

            // then
            Assert.That(tomasz.foaf.nick, Is.Empty);
        }

        [Test]
        public void Creating_Entity_should_allow_associated_Entity_subjects()
        {
            // given
            LoadTestFile("AssociatedInstances.trig");

            // when
            dynamic tomasz = EntityContext.Load(new EntityId("http://magi/people/Tomasz"));

            // then
            var entity=tomasz.foaf.first_knows;
            Assert.That(entity, Is.TypeOf<Entity>());
            Assert.That(entity.Id, Is.EqualTo(new EntityId("http://magi/people/Karol")));
        }

        [Test]
        public void Associated_Entity_should_allow_fetching_its_properties()
        {
            // given
            LoadTestFile("AssociatedInstances.trig");

            // when
            dynamic tomasz = EntityContext.Load(new EntityId("http://magi/people/Tomasz"));
            dynamic karol = tomasz.foaf.single_knows;

            // then
            Assert.That(karol.foaf.single_givenName, Is.EqualTo("Karol"));
        }

        [Test]
        public void Created_Entity_should_allow_using_unambiguous_predicates_without_prefix()
        {
            // given
            LoadTestFile("TriplesWithLiteralSubjects.trig");

            // when
            dynamic tomasz = EntityContext.Load(new EntityId("http://magi/people/Tomasz"));

            // then
            Assert.That(tomasz.givenName[0], Is.EqualTo("Tomasz"));
        }

        [Test]
        public void Should_throw_when_accessing_ambiguous_property()
        {
            // given
            LoadTestFile("TriplesWithLiteralSubjects.trig");

            // when
            dynamic tomasz = EntityContext.Load(new EntityId("http://magi/people/Tomasz"));

            // then
            var exception = Assert.Throws<AmbiguousPropertyException>(() => { var nick = tomasz.nick; });
            Assert.That(exception.Message, Contains.Substring("foaf:nick").And.ContainsSubstring("dummy:nick"));
        }

        [Test]
        public void Created_Entity_should_retrieve_triples_from_Union_Graph()
        {
            // given
            LoadTestFile("TriplesInNamedGraphs.trig");

            // when
            dynamic tomasz = EntityContext.Load(new EntityId("http://magi/people/Tomasz"));

            // then
            Assert.That(tomasz.knows != null);
            Assert.That(tomasz.knows.Count, Is.EqualTo(2));
            Assert.That(tomasz.knows[0], Is.InstanceOf<Entity>());
            Assert.That(tomasz.single_givenName != null);
            Assert.That(tomasz.single_givenName, Is.EqualTo("Tomasz"));
        }

        [Test]
        public void Should_read_blank_node_associated_Entities()
        {
            // given
            LoadTestFile("BlankNodes.trig");

            // when
            dynamic tomasz = EntityContext.Load(new EntityId("http://magi/people/Tomasz"));

            // then
            var entity=tomasz.single_knows;
            Assert.That(entity != null);
            Assert.That(entity, Is.InstanceOf<Entity>());
            Assert.That(entity.Id, Is.InstanceOf<BlankId>());
            Assert.That(entity.single_givenName != null);
            Assert.That(entity.single_givenName, Is.EqualTo("Karol"));
        }

        [Test]
        public void Should_read_rdf_lists_as_collection_of_Entities()
        {
            // given
            LoadTestFile("RdfLists.meta.ttl", new Uri("http://app.magi/graphs"));
            LoadTestFile("RdfLists.tomasz.ttl", new Uri("http://data.magi/people/Tomasz"));

            // when
            dynamic tomasz = EntityContext.Load(new EntityId("http://magi/people/Tomasz"));
            dynamic people = tomasz.knows;

            // then
            Assert.That(people.Count, Is.EqualTo(5));
            Assert.That(people[0].Id.Equals(new EntityId("http://magi/people/Karol")));
            Assert.That(people[1].Id.Equals(new EntityId("http://magi/people/Gniewko")));
            Assert.That(people[2].Id.Equals(new EntityId("http://magi/people/Monika")));
            Assert.That(people[3].Id.Equals(new EntityId("http://magi/people/Dominik")));
            Assert.That(people[4].Id.Equals(new EntityId("http://magi/people/Przemek")));
        }

        [Test]
        public void Should_read_rdf_lists_as_collection_of_literals()
        {
            // given
            LoadTestFile("RdfLists.meta.ttl", new Uri("http://app.magi/graphs"));
            LoadTestFile("RdfLists.tomasz.ttl", new Uri("http://data.magi/people/Tomasz"));

            // when
            dynamic tomasz = EntityContext.Load(new EntityId("http://magi/people/Tomasz"));
            dynamic people = tomasz.foaf.mbox;

            // then
            Assert.That(people.Count, Is.EqualTo(2));
            Assert.That(people[0], Is.EqualTo("tomasz.pluskiewicz@makolab.net"));
            Assert.That(people[1], Is.EqualTo("tomasz.pluskiewicz@makolab.pl"));
        }

        [Test]
        public void Should_allow_reading_nested_rdf_lists_as_collection_of_lists()
        {
            // given
            LoadTestFile("RdfLists.meta.ttl", new Uri("http://app.magi/graphs"));
            LoadTestFile("RdfLists.math.ttl", new Uri("urn:test:array"));

            // when
            dynamic tomasz = EntityContext.Load(new EntityId("http://magi/math/array"));
            dynamic numbers = tomasz.math.Original_matrix[0];

            // then
            Assert.That(numbers != null);
            Assert.That(numbers.Count, Is.EqualTo(2));
            ((IList)numbers[0]).Should().ContainInOrder(0L, 1L, 2L);
            ((IList)numbers[1]).Should().ContainInOrder(3L, 4L, 5L);
        }

        [Test]
        public void Should_read_rdf_lists_which_dont_use_blank_nodes()
        {
            // given
            LoadTestFile("RdfLists.meta.ttl", new Uri("http://app.magi/graphs"));
            LoadTestFile("RdfLists.tomasz.ttl", new Uri("http://data.magi/people/Tomasz"));

            // when
            dynamic tomasz = EntityContext.Load(new EntityId("http://magi/people/Tomasz"));
            dynamic nicks = tomasz.foaf.nick;

            // then
            Assert.That(nicks.Count, Is.EqualTo(2));
            Assert.That(nicks[0], Is.EqualTo("Tomasz"));
            Assert.That(nicks[1], Is.EqualTo("Tomek"));
        }
	}
}