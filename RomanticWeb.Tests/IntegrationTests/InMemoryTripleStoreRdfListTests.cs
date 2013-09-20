using System.Collections;
using FluentAssertions;
using ImpromptuInterface;
using NUnit.Framework;
using RomanticWeb.Entities;

namespace RomanticWeb.Tests.IntegrationTests
{
	[TestFixture]
	public class InMemoryTripleStoreRdfListTests : InMemoryTripleStoreTestsBase
	{
		[Test]
		public void Should_read_rdf_lists_as_collection_of_Entities()
		{
			// given
			LoadTestFile("RdfLists.ttl");

			// when
			dynamic tomasz = EntityFactory.Create(new EntityId("http://magi/people/Tomasz"));
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
			LoadTestFile("RdfLists.ttl");

			// when
			dynamic tomasz = EntityFactory.Create(new EntityId("http://magi/people/Tomasz"));
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
			LoadTestFile("RdfLists.ttl");

			// when
			dynamic tomasz = EntityFactory.Create(new EntityId("http://magi/test/array"));
			var people = (IList)tomasz.math.matrix;

			// then
			Assert.That(people != null);
			Assert.That(people.Count, Is.EqualTo(2));
			people[0].ActLike<IList>().Should().ContainInOrder(0, 1, 2);
			people[1].ActLike<IList>().Should().ContainInOrder(3, 4, 5);
		}
	}
}