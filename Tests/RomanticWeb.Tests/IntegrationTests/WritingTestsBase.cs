using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RomanticWeb.Entities;
using RomanticWeb.TestEntities.Foaf;
using RomanticWeb.Tests.Helpers;
using RomanticWeb.Tests.Stubs;
using RomanticWeb.Vocabularies;
using VDS.RDF.Query.Builder;

namespace RomanticWeb.Tests.IntegrationTests
{
    public abstract class WritingTestsBase : IntegrationTestsBase
    {
        protected abstract int MetagraphTripleCount { get; }

        protected abstract int AllTriplesCount { get; }

        [Test]
        public void Should_commit_uri_node()
        {
            // given

            // when
            IAgent entity = EntityContext.Create<IAgent>("http://magi/people/Tomasz");
            EntityContext.Commit();

            // then
            AssertStoreCounts(1, 1);
        }

        [Test]
        public void Should_commit_literal_node()
        {
            // given

            // when
            IAgent entity = EntityContext.Create<IAgent>("http://magi/people/Tomasz");
            entity.Gender = "male";
            EntityContext.Commit();

            // then
            AssertStoreCounts(2, 1);
        }

        [Test]
        public void Should_commit_blank_node()
        {
            // given

            // when
            IAgent entity = EntityContext.Create<IAgent>("http://magi/people/Tomasz");
            entity.KnowsOne = EntityContext.Create<IAgent>(entity.CreateBlankId());
            EntityContext.Commit();

            // then
            AssertStoreCounts(3, 1);
        }

        [Test]
        public void Should_remove_uri_node()
        {
            // given
            IAgent entity = EntityContext.Create<IAgent>("http://magi/people/Tomasz");
            EntityContext.Commit();

            // when
            EntityContext.Delete(entity.Id);
            EntityContext.Commit();

            // then
            AssertStoreCounts(0, 0);
        }

        [Test]
        public void Should_remove_literal_node()
        {
            // given
            IAgent entity = EntityContext.Create<IAgent>("http://magi/people/Tomasz");
            entity.Gender = "male";
            EntityContext.Commit();

            // when
            entity.Gender = null;
            EntityContext.Commit();

            // then
            AssertStoreCounts(1, 1);
        }

        [Test]
        public void Should_remove_blank_node()
        {
            // given
            IAgent entity = EntityContext.Create<IAgent>("http://magi/people/Tomasz");
            entity.KnowsOne = EntityContext.Create<IAgent>(entity.CreateBlankId());
            EntityContext.Commit();

            // when
            EntityContext.Delete(entity.KnowsOne.Id);
            EntityContext.Commit();

            // then
            AssertStoreCounts(1, 1);
        }

        [Test]
        public void Should_remove_whole_entity_graph()
        {
            // given
            IAgent entity = EntityContext.Create<IAgent>("http://magi/people/Tomasz");
            entity.KnowsOne = EntityContext.Create<IAgent>(entity.CreateBlankId());
            entity.Gender = "male";
            IAgent someEntity = EntityContext.Create<IAgent>("http://magi/people/Karol");
            someEntity.KnowsOne = entity;
            EntityContext.Commit();

            // when
            EntityContext.Delete(entity.Id, DeleteBehaviour.DeleteChildren | DeleteBehaviour.NullifyChildren);
            EntityContext.Commit();

            // then
            AssertStoreCounts(1, 1);
        }

        [Test]
        public void Should_reconstruct_entity()
        {
            // given
            IAlsoAgent entity = EntityContext.Create<IAlsoAgent>("http://magi/people/Tomasz");
            IAgent someAgent = EntityContext.Create<IAgent>(entity.CreateBlankId());
            IAgent anotherAgent = EntityContext.Create<IAgent>(entity.CreateBlankId());
            entity.Knows.Add(someAgent);
            entity.Knows.Add(anotherAgent);
            EntityContext.Commit();

            // when
            EntityContext.Delete(someAgent.Id, DeleteBehaviour.DeleteChildren | DeleteBehaviour.NullifyChildren);
            EntityContext.Commit();

            // then
            AssertStoreCounts(6, 1);
        }

        [Test]
        public void Should_correctly_delete_and_create_an_entity()
        {
            // given 
            LoadTestFile("AssociatedInstances.trig");

            // when
            EntityContext.Delete(new Uri("http://magi/people/Karol"));
            var person = EntityContext.Create<IPerson>(new Uri("http://magi/people/Karol"));
            person.Name = "Charles";
            EntityContext.Commit();

            // then
            EntityContext.Store.Quads.Where(q => q.Graph.Uri == new Uri("http://data.magi/people/Karol"))
                         .Should().HaveCount(3);
        }

        [Test]
        public void Should_delete_entity_even_if_wasnt_loaded()
        {
            // given 
            LoadTestFile("AssociatedInstances.trig");

            // when
            EntityContext.Delete(new Uri("http://magi/people/Karol"));
            EntityContext.Commit();

            // then
            EntityContext.Store.Quads.Should().HaveCount(0);
            Store.Should().NotMatchAsk(b => b.Subject(new Uri("http://magi/people/Karol")).Predicate("p").Object("o"));
        }

        [Test]
        public void Should_retain_changes_to_entity_initially_deleted()
        {
            // given
            var entityId = new Uri("http://magi/people/Karol"); 
            LoadTestFile("AssociatedInstances.trig");

            // when
            EntityContext.Delete(entityId);
            var charles = EntityContext.Create<IGroup>(entityId);
            charles.Gender = "male";
            EntityContext.Commit();

            // then
            EntityContext.Store.Quads.Should().HaveCount(3);
            Store.Should().MatchAsk(b =>
                b.Subject(entityId).PredicateUri(Foaf.gender).Object("gender")
                 .Subject(entityId).PredicateUri(Rdf.type).Object(Foaf.Agent)
                 .Subject(entityId).PredicateUri(Rdf.type).Object(Foaf.Group));
        }

        protected override void ChildSetup()
        {
            Factory.WithNamedGraphSelector(new TestGraphSelector());
        }

        private void AssertStoreCounts(int dataQuadsCount, int metagraphQuads)
        {
            EntityContext.Store.Quads.Should().HaveCount(dataQuadsCount);
            MetagraphTripleCount.Should().Be(metagraphQuads);
            AllTriplesCount.Should().Be(metagraphQuads + dataQuadsCount);
        }
    }
}