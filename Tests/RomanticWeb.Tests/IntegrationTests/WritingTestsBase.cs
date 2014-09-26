using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RomanticWeb.Entities;
using RomanticWeb.TestEntities.Foaf;
using RomanticWeb.Tests.Helpers;
using RomanticWeb.Tests.Linq;
using RomanticWeb.Tests.Stubs;
using RomanticWeb.Vocabularies;
using VDS.RDF;
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
        public void Should_reconstruct_graph_when_deleting_blank()
        {
            // given
            Uri tomasz = new Uri("http://magi/people/Tomasz");
            IAlsoAgent entity = EntityContext.Create<IAlsoAgent>(tomasz);
            IAgent someAgent = EntityContext.Create<IAgent>(entity.CreateBlankId());
            IAgent anotherAgent = EntityContext.Create<IAgent>(entity.CreateBlankId());
            entity.Knows.Add(someAgent);
            entity.Knows.Add(anotherAgent);
            EntityContext.Commit();

            // when
            entity.Knows.Remove(someAgent);
            EntityContext.Delete(someAgent.Id, DeleteBehaviour.DeleteChildren | DeleteBehaviour.NullifyChildren);
            EntityContext.Commit();

            // then
            AssertStoreCounts(5, 1);
            IGraph dataGraph = Store[new Uri("http://data.magi/people/Tomasz")];
            INode root = dataGraph.GetTriplesWithPredicate(Foaf.knows).Single().Object;
            dataGraph.GetListItems(root).Should().HaveCount(1);
        }

        [Test]
        public void Should_reconstruct_entity2()
        {
            // given
            var entity = EntityContext.Create<IAgent>("http://magi/people/Tomasz");
            var someAgent = EntityContext.Create<IAgent>(entity.CreateBlankId());
            someAgent.Gender = "male";
            entity.KnowsOne = someAgent;
            EntityContext.Commit();

            // when
            var anotherAgent = EntityContext.Create<IAgent>(entity.CreateBlankId());
            anotherAgent.Gender = "female";
            entity.KnowsOne = anotherAgent;
            EntityContext.Commit();

            // then
            AssertStoreCounts(4, 1);
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

        [Test]
        public void Must_not_update_meta_graph_with_blank_entity_id()
        {
            // given
            var entity = EntityContext.Create<IAgent>("http://magi/people/Tomasz");
            var someAgent = EntityContext.Create<IAgent>(entity.CreateBlankId());
            entity.KnowsOne = someAgent;
            EntityContext.Commit();

            // when
            someAgent.Gender = "male";
            EntityContext.Commit();

            // then
            MetagraphTripleCount.Should().Be(1);
        }

        [Test]
        public void Should_write_correct_data_when_recreating_entity_with_blank_node()
        {
            // given
            var entityId = new Uri("http://magi/people/Tomasz");
            LoadTestFile("BlankNodes.trig");
            
            // when
            EntityContext.Delete(entityId);
            var person = EntityContext.Create<IPerson>(entityId);
            var friend = EntityContext.Create<IPerson>(person.CreateBlankId());
            person.KnowsOne = friend;
            friend.Name = "Karol";
            EntityContext.Commit();

            // then
            Store.Should().MatchAsk(
                b => b.Subject(entityId).PredicateUri(Foaf.knows).Object("blank")
                      .Subject("blank").PredicateUri(Foaf.givenName).Object("name"),
                f => f.Str(f.Variable("name")) == "Karol" && f.IsBlank("blank"));
        }

        [Test]
        public void Should_not_loose_triples_when_setting_same_blank()
        {
            // given
            var entityId = new Uri("http://magi/people/Tomasz");
            LoadTestFile("BlankNodes.trig");

            // when
            var person = EntityContext.Load<IPerson>(entityId);
            var friend = person.KnowsOne;
            person.KnowsOne = friend;

            // then
            Store.Should().MatchAsk(
                b => b.Subject(entityId).PredicateUri(Foaf.knows).Object("blank")
                      .Subject("blank").PredicateUri(Foaf.givenName).Object("name")
                      .Subject("blank").PredicateUri(Foaf.knows).Object("nextBlank")
                      .Subject("nextBlank").PredicateUri(Foaf.givenName).Object("secondName"),
                f => f.Str(f.Variable("name")) == "Karol" && f.IsBlank("blank")
                  && f.Str(f.Variable("secondName")) == "Gniewosław" && f.IsBlank("nextBlank"));
        }

        [Test]
        public void Should_recreate_graph_with_complete_data()
        {
            // given
            var entityId = new Uri("http://magi/people/Tomasz");
            LoadTestFile("BlankNodes.trig");

            // when
            var person = EntityContext.Load<IPerson>(entityId);
            var friend = EntityContext.Create<IPerson>(person.CreateBlankId());
            friend.Name = "Jan";
            person.Knows = new[] { friend };
            EntityContext.Commit();

            // then
            Store.Triples.Should().HaveCount(7);
            Store.Should().MatchAsk(
                b => b.Subject(entityId).PredicateUri(Foaf.knows).Object("blank")
                      .Subject("blank").PredicateUri(Foaf.givenName).Object("name"),
                f => f.Str(f.Variable("name")) == "Jan" && f.IsBlank("blank"));
        }

        [Test]
        public void Should_allow_modifying_retrieved_blank_nodes()
        {
            // given
            EntityId hniewo = new Uri("http://magi/people/Gniewoslaw");
            LoadTestFile("TriplesWithLiteralSubjects.trig");
            var address = (from resources in EntityContext.AsQueryable<IPerson>()
                           where resources.Id == hniewo
                           select resources.Address).Single();

            // when
            address.Street = "Demokratyczna 46";
            EntityContext.Commit();

            // then
            Store.Should().MatchAsk(
                tb => tb.Subject(hniewo.Uri).PredicateUri("schema:address").Object("addr")
                        .Subject("addr").PredicateUri("schema:streetAddress").Object("street"),
                eb => eb.Str(eb.Variable("street")) == "Demokratyczna 46");
        }

        protected override void ChildSetup()
        {
            Factory.WithNamedGraphSelector(new TestGraphSelector());
        }

        private void AssertStoreCounts(int dataQuadsCount, int metagraphQuads)
        {
            EntityContext.Store.Quads.Should().HaveCount(dataQuadsCount, "that's how many quads internal store should contain");
            MetagraphTripleCount.Should().Be(metagraphQuads, "that's how many quads meta graph should contain");
            AllTriplesCount.Should().Be(metagraphQuads + dataQuadsCount, "that's how many quads external store should contain");
        }
    }
}