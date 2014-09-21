using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Mapping.Sources;
using RomanticWeb.Model;
using RomanticWeb.TestEntities;
using RomanticWeb.Tests.IntegrationTests.TestMappings;
using RomanticWeb.Tests.Stubs;
using RomanticWeb.Updates;
using RomanticWeb.Vocabularies;
using IPerson = RomanticWeb.TestEntities.IPerson;

namespace RomanticWeb.Tests.IntegrationTests
{
    public abstract class MappingTestsBase : IntegrationTestsBase
    {
        private static readonly EntityId EntityId = new EntityId("http://magi/people/Tomasz");

        protected IPerson Entity
        {
            get { return EntityContext.Load<IPerson>(EntityId); }
        }

        private new TestMappingSource Mappings
        {
            get { return (TestMappingSource)base.Mappings; }
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
            Factory.WithNamedGraphSelector(new ProtocolReplaceGraphSelector());
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
            Mappings.Add(new DefaultGraphPersonMapping(true));
            LoadTestFile("RdfLists.trig");

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
                   .Should().Contain(new[] { "Karol", "Gniewko", "Monika", "Dominik", "Przemek" });
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
        public void Mapping_URI_node_rdf_collection_of_entities_should_be_possible()
        {
            // given
            Mappings.Add(new DefaultGraphPersonMapping(true));
            LoadTestFile("RdfLists.trig");

            // when
            var friends = Entity.NickNames;

            // then
            Assert.That(friends, Has.Count.EqualTo(2));
            friends.Should().Contain("Tomek", "Tomasz");
        }

        [Test]
        public void Mapping_URI_node_rdf_collection_of_literals_should_be_possible()
        {
            // given
            Mappings.Add(new DefaultGraphPersonMapping(true));
            LoadTestFile("RdfLists.trig");

            // when
            var friends = Entity.FriendsAsLiterals;

            // then
            Assert.That(friends, Has.Count.EqualTo(5));
        }

        [Test]
        public void Should_throw_if_a_property_returns_multiple_values()
        {
            Mappings.Add(new DefaultGraphPersonMapping());
            LoadTestFile("Collections.trig");

            // then
            var cardinalityException = Assert.Throws<CardinalityException>(() => { var hp = Entity.FirstName; });
            Assert.That(cardinalityException.ExpectedCardinality, Is.EqualTo(1));
            Assert.That(cardinalityException.ActualCardinality, Is.EqualTo(2));
        }

        [Test]
        public void Should_throw_if_a_collection_is_backed_by_a_list_and_a_direct_relation()
        {
            Mappings.Add(new DefaultGraphPersonMapping());
            LoadTestFile("MixedCollections.trig");

            // then
            var cardinalityException = Assert.Throws<CardinalityException>(() => { var hp = Entity.HomepageAsEntities; });
            Assert.That(cardinalityException.ExpectedCardinality, Is.EqualTo(1));
            Assert.That(cardinalityException.ActualCardinality, Is.EqualTo(2));
        }

        [Test]
        public void Should_throw_if_a_collection_is_backed_by_a_list_and_a_direct_relation_2()
        {
            Mappings.Add(new DefaultGraphPersonMapping());
            LoadTestFile("MixedCollections.trig");

            // then
            var cardinalityException = Assert.Throws<ArgumentOutOfRangeException>(() => { var hp = Entity.Homepage; });
            Assert.That(cardinalityException.ParamName, Is.EqualTo("objectNode"));
        }

        [Test]
        public void Should_throw_if_a_collection_is_backed_by_two_lists()
        {
            Mappings.Add(new DefaultGraphPersonMapping());
            LoadTestFile("MixedCollections.trig");

            // then
            Assert.Throws<CardinalityException>(() => { var hp = Entity.HomepageAsEntities; });
        }

        [Test]
        public void Setting_property_should_be_possible_in_default_graph()
        {
            // given
            Mappings.Add(new DefaultGraphPersonMapping());
            LoadTestFile("TriplesWithLiteralSubjects.trig");

            // when
            Entity.FirstName = "Michał";

            // then
            Assert.That(Entity.FirstName, Is.EqualTo("Michał"));
            Assert.That(EntityStore.Quads, Has.Count.EqualTo(6));
        }

        [Test]
        public void Setting_property_should_be_possible_in_named_graph()
        {
            // given
            Factory.WithNamedGraphSelector(new ProtocolReplaceGraphSelector());
            Mappings.Add(new NamedGraphsPersonMapping());
            LoadTestFile("TriplesInNamedGraphs.trig");

            // when
            Entity.FirstName = "Dominik";
            Entity.LastName = "Kuziński";

            // then
            Assert.That(Entity.FirstName, Is.EqualTo("Dominik"));
            Assert.That(Entity.LastName, Is.EqualTo("Kuziński"));
            Assert.That(EntityStore.Quads, Has.Count.EqualTo(5), "Actual triples were: {0}", SerializeStore());
            var quads = EntityStore.Quads.Where(q => q.Graph == Node.ForUri(new Uri("personal://magi/people/Tomasz")));
            Assert.That(quads.ToList(), Has.Count.EqualTo(2), "Actual triples were: {0}", SerializeStore());
        }

        [Test]
        public void Setting_rdf_list_of_literals_should_be_possible()
        {
            // given
            Factory.WithNamedGraphSelector(new ProtocolReplaceGraphSelector());
            Mappings.Add(new NamedGraphsPersonMapping());
            LoadTestFile("TriplesInNamedGraphs.trig");

            // when
            Entity.Interests = new[] { "semantic web", "linked data" };

            // then
            Entity.Interests.Should().ContainInOrder("semantic web", "linked data");
            var quads = EntityStore.Quads.Where(q => q.Graph == Node.ForUri(new Uri("interestsOf://magi/people/Tomasz")));
            Assert.That(quads.ToList(), Has.Count.EqualTo(5), "Actual triples were: {0}", SerializeStore());
        }

        [Test]
        public void Setting_entity_property_should_be_posible()
        {
            // given
            Mappings.Add(new DefaultGraphPersonMapping());
            LoadTestFile("TriplesInNamedGraphs.trig");
            var someEntity = EntityContext.Create<IEntity>(new EntityId("urn:possibly:external"));
            Entity.ForceInitialize();
            var quadsInitially = EntityStore.Quads.Count();

            // when
            Entity.Entity = someEntity;

            // then
            Assert.That(Entity.Entity, Is.Not.Null);
            Assert.That(Entity.Entity.Id, Is.EqualTo(new EntityId("urn:possibly:external")));
            Assert.That(EntityStore.Quads.Count(), Is.EqualTo(quadsInitially + 1), "Actual triples were: {0}", SerializeStore());
        }

        [Test]
        public void Setting_simple_collection_of_literals_should_be_possible()
        {
            // given
            Mappings.Add(new DefaultGraphPersonMapping());
            LoadTestFile("TriplesInNamedGraphs.trig");
            Entity.ForceInitialize();
            var quadsInitially = EntityStore.Quads.Count();

            // when
            Entity.Interests = new[] { "semantic web", "linked data" };

            // then
            Assert.That(Entity.Interests, Contains.Item("semantic web"));
            Assert.That(Entity.Interests, Contains.Item("linked data"));
            Assert.That(EntityStore.Quads.Count(), Is.EqualTo(quadsInitially + 2), "Actual triples were: {0}", SerializeStore());
        }

        [Test]
        public void Setting_simple_collection_of_entities_should_be_possible()
        {
            // given
            Mappings.Add(new DefaultGraphPersonMapping());
            LoadTestFile("TriplesInNamedGraphs.trig");
            var someEntity = EntityContext.Create<IPerson>(new EntityId("urn:possibly:friend1"));
            var otherEntity = EntityContext.Create<IPerson>(new EntityId("urn:possibly:friend2"));
            Entity.ForceInitialize();
            var quadsInitially = EntityStore.Quads.Count();

            // when
            Entity.Friends = new[] { someEntity, otherEntity };

            // then
            Assert.That(Entity.Friends, Contains.Item(someEntity));
            Assert.That(Entity.Friends, Contains.Item(otherEntity));
            Assert.That(EntityStore.Quads.Count(), Is.EqualTo(quadsInitially + 4), "Actual triples were: {0}", SerializeStore());
        }

        [Test]
        public void Should_allow_chaining_typed_entities()
        {
            // given
            Mappings.Add(new DefaultGraphPersonMapping());
            LoadTestFile("AssociatedInstances.trig");

            // when
            var karol = Entity.Friend;

            // then
            Assert.That(karol, Is.InstanceOf<IPerson>());
            Assert.That(karol.FirstName, Is.EqualTo("Karol"));
        }

        [Test]
        public void Creating_entity_should_populate_changeset_with_all_entity_types()
        {
            // given
            Mappings.Add(new DefaultGraphPersonMapping());
            EntityId entityId = "http://magi/people/Tomasz";
            EntityId graphUri = "http://data.magi/people/Tomasz";

            // then
            EntityContext.Create<TestEntities.Foaf.IPerson>(entityId);

            // then
            EntityContext.Changes.Should().Contain(change => GraphUpdateSettingRdfTypes(change));
        }

        [Test]
        public void Should_only_return_triples_from_the_selected_named_graph()
        {
            // given
            Mappings.Add(new DefaultGraphPersonMapping());
            LoadTestFile("TriplesInUnmappedGraphs.trig");

            // when
            var name = Entity.FirstName;

            // then
            Assert.That(name, Is.Null);
        }

        [Test]
        public void Given_new_entity_should_allow_adding_entity_to_collection()
        {
            // given
            Mappings.Add(new DefaultGraphPersonMapping());
            var tomasz = EntityContext.Create<IPerson>(new EntityId("urn:person:tomasz"));
            var gniewo = EntityContext.Create<IPerson>(new EntityId("urn:person:gniewo"));

            // when
            tomasz.Friends.Add(gniewo);

            // then
            tomasz.Friends.Should().Contain(gniewo);
        }

        [Test]
        public void Mapped_collection_of_entites_should_allow_removing_members()
        {
            // given
            Mappings.Add(new DefaultGraphPersonMapping());
            LoadTestFile("LooseCollections.trig");

            // when
            Entity.Friends.Remove(EntityContext.Load<IPerson>(new EntityId("http://magi/people/Monika")));
            var friends = Entity.Friends;

            // then
            Assert.That(friends, Has.Count.EqualTo(4));
            friends.Select(friend => friend.FirstName)
                   .Should().Contain(new[] { "Karol", "Gniewko", "Dominik", "Przemek" });
        }

        [Test]
        public void Setting_literal_should_convert_to_correct_node()
        {
            // given
            Factory.WithNamedGraphSelector(new ProtocolReplaceGraphSelector());
            Mappings.Add(new NamedGraphsPersonMapping());
            LoadTestFile("TriplesInNamedGraphs.trig");

            // when
            Entity.Age = 30;

            // then
            Assert.That(Entity.Age, Is.EqualTo(30));
            (from quad in EntityContext.Store.Quads
             where quad.Predicate == Node.ForUri(new Uri("http://xmlns.com/foaf/0.1/age"))
             select quad).ToList().Should().OnlyContain(q => q.Object.DataType == Xsd.Int);
        }

        [Test]
        public void Should_retrieve_value_from_interface_inherited_property()
        {
            LoadTestFile("InheritedPropertyGraph.trig");
            var entity = EntityContext.Load<TestEntities.Inheritance.Specialized.IInterface>(new EntityId(new Uri("http://test.org/test")));
            entity.Description = "test";
            Assert.That(entity.Description, Is.EqualTo("test"));
        }

        [Test]
        public void Should_allow_adding_element_to_empty_list()
        {
            // given
            var entityId = new EntityId("urn:test:entity");
            var entity = EntityContext.Create<IEntityWithCollections>(entityId);

            // when
            entity.DefaultListMapping.Add("test string");

            // then
            entity.DefaultListMapping.Should().HaveCount(1);
            entity.DefaultListMapping.Should().Contain("test string");
            EntityStore.Quads.Should().HaveCount(3, "Actual triples were: {0}", SerializeStore());
            EntityStore.Quads.Should().Contain(quad => quad.Subject.IsBlank
                                                    && quad.Predicate == Node.ForUri(Rdf.first)
                                                    && quad.Object.IsLiteral
                                                    && quad.Object.Literal == "test string");
            EntityStore.Quads.Should().Contain(quad => quad.Subject == Node.FromEntityId(entityId)
                                                    && AbsoluteUriComparer.Default.Compare(quad.Predicate.Uri, new Uri("http://magi/ontology#collection")) == 0
                                                    && quad.Object.IsBlank);
            EntityStore.Quads.Should().Contain(quad => quad.Subject.IsBlank
                                                    && quad.Predicate == Node.ForUri(Rdf.rest)
                                                    && quad.Object == Node.ForUri(Rdf.nil));
        }

        [Test]
        public void Should_allow_adding_element_to_existing_collection()
        {
            // given
            LoadTestFile("BlankNodes.trig");
            var entity = EntityContext.Load<TestEntities.Foaf.IPerson>(EntityId);

            // when
            var newFriend = EntityContext.Create<TestEntities.Foaf.IPerson>("urn:other:friend");
            newFriend.Name = "Dominik";
            entity.Knows.Add(newFriend);

            // then
            entity.Knows.Should().HaveCount(2);
            EntityStore.Quads.Should().HaveCount(13, "Should contain preexisting blank node subgraphs. Actual triples were: {0}", SerializeStore());
        }

        [Test]
        public void Should_allow_replacing_existing_collection()
        {
            // given
            LoadTestFile("BlankNodes.trig");
            var entity = EntityContext.Load<TestEntities.Foaf.IPerson>(EntityId);

            // when
            var newFriend = EntityContext.Create<TestEntities.Foaf.IPerson>("urn:new:friend");
            var otherNewFriend = EntityContext.Create<TestEntities.Foaf.IPerson>("urn:other:friend");
            entity.Knows = new[] { newFriend, otherNewFriend };

            // then
            entity.Knows.Should().HaveCount(2);
            EntityStore.Quads.Should().HaveCount(8, "Should not contain preexisting blank node subgraphs. Actual triples were: {0}", SerializeStore());
        }

        protected override IMappingProviderSource SetupMappings()
        {
            return new TestMappingSource();
        }

        protected override void ChildSetup()
        {
            Factory.WithNamedGraphSelector(new TestGraphSelector());
        }

        private string SerializeStore()
        {
            return String.Join(Environment.NewLine, EntityStore.Quads);
        }

        private static bool GraphUpdateSettingRdfTypes(DatasetChange change)
        {
            var update = change as GraphUpdate;
            return update != null
                && update.AddedQuads.All(q => q.Predicate == Node.ForUri(Rdf.type))
                && (update.AddedQuads.Any(q => q.Object == Node.ForUri(Foaf.Person))
                || update.AddedQuads.Any(q => q.Object == Node.ForUri(Foaf.Agent)));
        }

        [Obsolete]
        private class ProtocolReplaceGraphSelector : RomanticWeb.NamedGraphs.INamedGraphSelector
        {
            public Uri SelectGraph(EntityId id, IEntityMapping entityMapping, IPropertyMapping predicate)
            {
                string replacementProtocol = "http";

                if (predicate != null && predicate.Uri != null)
                {
                    switch (predicate.Uri.ToString())
                    {
                        case "http://xmlns.com/foaf/0.1/familyName":
                        case "http://xmlns.com/foaf/0.1/givenName":
                            replacementProtocol = "personal";
                            break;
                        case "http://xmlns.com/foaf/0.1/knows":
                            replacementProtocol = "friendsOf";
                            break;
                        case "http://xmlns.com/foaf/0.1/homePage":
                        case "http://xmlns.com/foaf/0.1/interest":
                            replacementProtocol = "interestsOf";
                            break;
                    }
                }

                return new Uri(id.ToString().Replace("http", replacementProtocol));
            }
        }
    }
}
