using System;
using System.Collections;
using FluentAssertions;
using ImpromptuInterface;
using NUnit.Framework;
using RomanticWeb.Entities;
using RomanticWeb.Tests.Helpers;

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
            dynamic tomasz = EntityContext.Create(new EntityId("http://magi/people/Tomasz"));

            // then
            Assert.That(tomasz.foaf.givenName, Is.EqualTo("Tomasz"));
            Assert.That(tomasz.foaf.familyName, Is.EqualTo("Pluskiewicz"));
            Assert.That(tomasz.foaf.nick == null);
        }

        [Test]
        public void Creating_Entity_should_return_null_when_triple_doesnt_exist()
        {
            // given
            LoadTestFile("TriplesWithLiteralSubjects.trig");

            // when
            dynamic tomasz = EntityContext.Create(new EntityId("http://magi/people/Tomasz"));

            // then
            Assert.That(tomasz.foaf.nick == null);
        }

        [Test]
        public void Creating_Entity_should_allow_associated_Entity_subjects()
        {
            // given
            LoadTestFile("AssociatedInstances.trig");

            // when
            dynamic tomasz = EntityContext.Create(new EntityId("http://magi/people/Tomasz"));

            // then
            Assert.That(tomasz.foaf.knows, Is.TypeOf<Entity>());
            Assert.That(tomasz.foaf.knows.Id, Is.EqualTo(new EntityId("http://magi/people/Karol")));
        }

        [Test]
        public void Associated_Entity_should_allow_fetching_its_properties()
        {
            // given
            LoadTestFile("AssociatedInstances.trig");

            // when
            dynamic tomasz = EntityContext.Create(new EntityId("http://magi/people/Tomasz"));
            dynamic karol = tomasz.foaf.knows;

            // then
            Assert.That(karol.foaf.givenName, Is.EqualTo("Karol"));
        }

        [Test]
        public void Created_Entity_should_allow_using_unambiguous_predicates_without_prefix()
        {
            // given
            LoadTestFile("TriplesWithLiteralSubjects.trig");

            // when
            dynamic tomasz = EntityContext.Create(new EntityId("http://magi/people/Tomasz"));

            // then
            Assert.That(tomasz.givenName, Is.EqualTo("Tomasz"));
        }

        [Test]
        public void Should_throw_when_accessing_ambiguous_property()
        {
            // given
            LoadTestFile("TriplesWithLiteralSubjects.trig");

            // when
            dynamic tomasz = EntityContext.Create(new EntityId("http://magi/people/Tomasz"));

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
            dynamic tomasz = EntityContext.Create(new EntityId("http://magi/people/Tomasz"));

            // then
            Assert.That(tomasz.knows != null);
            Assert.That(tomasz.knows.Count, Is.EqualTo(2));
            Assert.That(tomasz.knows[0], Is.InstanceOf<Entity>());
            Assert.That(tomasz.givenName != null);
            Assert.That(tomasz.givenName, Is.EqualTo("Tomasz"));
        }

        [Test]
        public void Should_read_blank_node_associated_Entities()
        {
            // given
            LoadTestFile("BlankNodes.trig");

            // when
            dynamic tomasz = EntityContext.Create(new EntityId("http://magi/people/Tomasz"));

            // then
            Assert.That(tomasz.knows != null);
            Assert.That(tomasz.knows, Is.InstanceOf<Entity>());
            Assert.That(tomasz.knows.Id, Is.InstanceOf<BlankId>());
            Assert.That(tomasz.knows.givenName != null);
            Assert.That(tomasz.knows.givenName, Is.EqualTo("Karol"));
        }

        [Test]
        public void Should_read_rdf_lists_as_collection_of_Entities()
        {
            // given
            LoadTestFile("RdfLists.meta.ttl", new Uri("http://app.magi/graphs"));
            LoadTestFile("RdfLists.tomasz.ttl", new Uri("http://data.magi/people/Tomasz"));

            // when
            dynamic tomasz = EntityContext.Create(new EntityId("http://magi/people/Tomasz"));
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
            dynamic tomasz = EntityContext.Create(new EntityId("http://magi/people/Tomasz"));
            dynamic people = tomasz.foaf.mbox;

            // then
            Assert.That(people.Count, Is.EqualTo(2));
            Assert.That(people[0], Is.EqualTo("tomasz.pluskiewicz@makolab.net"));
            Assert.That(people[1], Is.EqualTo("tomasz.pluskiewicz@makolab.pl"));
        }

        [Test]
        public void Should_read_nested_rdf_lists_as_collection_of_lists()
        {
            // given
            LoadTestFile("RdfLists.meta.ttl", new Uri("http://app.magi/graphs"));
            LoadTestFile("RdfLists.math.ttl", new Uri("urn:test:array"));

            // when
            dynamic tomasz = EntityContext.Create(new EntityId("http://magi/math/array"));
            var numbers = (IList)tomasz.math.matrix;

            // then
            Assert.That(numbers != null);
            Assert.That(numbers.Count, Is.EqualTo(2));
            numbers[0].ActLike<IList>().Should().ContainInOrder(0, 1, 2);
            numbers[1].ActLike<IList>().Should().ContainInOrder(3, 4, 5);
        }
	}
}