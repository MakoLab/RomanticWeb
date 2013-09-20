using NUnit.Framework;
using RomanticWeb.Entities;

namespace RomanticWeb.Tests.IntegrationTests
{
	[TestFixture]
	public class InMemoryTripleStoreBasicTests : InMemoryTripleStoreTestsBase
	{
		[Test]
		public void Creating_Entity_should_allow_accessing_existing_literal_properties()
		{
			// given
			LoadTestFile("TriplesWithLiteralSubjects.ttl");

			// when
			dynamic tomasz = EntityFactory.Create(new EntityId("http://magi/people/Tomasz"));

			// then
			Assert.That(tomasz.foaf.givenName, Is.EqualTo("Tomasz"));
			Assert.That(tomasz.foaf.familyName, Is.EqualTo("Pluskiewicz"));
			Assert.That(tomasz.foaf.nick == null);
		}

		[Test]
		public void Creating_Entity_should_return_null_when_triple_doesnt_exist()
		{
			// given
			LoadTestFile("TriplesWithLiteralSubjects.ttl");

			// when
			dynamic tomasz = EntityFactory.Create(new EntityId("http://magi/people/Tomasz"));

			// then
			Assert.That(tomasz.foaf.nick == null);
		}

		[Test]
		public void Creating_Entity_should_allow_associated_Entity_subjects()
		{
			// given
			LoadTestFile("AssociatedInstances.ttl");

			// when
			dynamic tomasz = EntityFactory.Create(new EntityId("http://magi/people/Tomasz"));

			// then
			Assert.That(tomasz.foaf.knows, Is.TypeOf<Entity>());
			Assert.That(tomasz.foaf.knows.Id, Is.EqualTo(new EntityId("http://magi/people/Karol")));
		}

		[Test]
		public void Associated_Entity_should_allow_fetching_its_properties()
		{
			// given
			LoadTestFile("AssociatedInstances.ttl");

			// when
			dynamic tomasz = EntityFactory.Create(new EntityId("http://magi/people/Tomasz"));
			dynamic karol = tomasz.foaf.knows;

			// then
			Assert.That(karol.foaf.givenName, Is.EqualTo("Karol"));
		}

		[Test]
		public void Created_Entity_should_allow_using_unambiguous_predicates_without_prefix()
		{
			// given
			LoadTestFile("TriplesWithLiteralSubjects.ttl");

			// when
			dynamic tomasz = EntityFactory.Create(new EntityId("http://magi/people/Tomasz"));

			// then
			Assert.That(tomasz.givenName, Is.EqualTo("Tomasz"));
		}

		[Test]
		public void Should_throw_when_accessing_ambiguous_property()
		{
			// given
			LoadTestFile("TriplesWithLiteralSubjects.ttl");

			// when
			dynamic tomasz = EntityFactory.Create(new EntityId("http://magi/people/Tomasz"));

			// then
			var exception = Assert.Throws<AmbiguousPropertyException>(() => { var nick = tomasz.nick; });
			Assert.That(exception.Message, Contains.Substring("foaf:nick").And.ContainsSubstring("dummy:nick"));
		}

		[Test]
		public void Created_Entiy_should_retrieve_triples_from_Union_Graph()
		{
			// given
			LoadTestFile("TriplesInNamedGraphs.trig");

			// when
			dynamic tomasz = EntityFactory.Create(new EntityId("http://magi/people/Tomasz"));

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
			LoadTestFile("BlankNodes.ttl");

			// when
			dynamic tomasz = EntityFactory.Create(new EntityId("http://magi/people/Tomasz"));

			// then
			Assert.That(tomasz.knows != null);
			Assert.That(tomasz.knows, Is.InstanceOf<Entity>());
			Assert.That(tomasz.knows.Id, Is.InstanceOf<BlankId>());
			Assert.That(tomasz.knows.givenName != null);
			Assert.That(tomasz.knows.givenName, Is.EqualTo("Karol"));
		}
	}
}