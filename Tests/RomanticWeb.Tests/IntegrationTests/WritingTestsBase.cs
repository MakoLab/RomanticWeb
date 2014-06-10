using FluentAssertions;
using NUnit.Framework;
using RomanticWeb.Entities;
using RomanticWeb.TestEntities.Foaf;
using RomanticWeb.Tests.Stubs;

namespace RomanticWeb.Tests.IntegrationTests
{
    public abstract class WritingTestsBase:IntegrationTestsBase
    {
        private static readonly EntityId EntityId=new EntityId("http://magi/people/Tomasz");

        [Test]
        public virtual void Should_commit_uri_node()
        {
            // given

            // when
            IAgent entity=EntityContext.Create<IAgent>("http://magi/people/Tomasz");
            EntityContext.Commit();

            // then
            EntityContext.Store.Quads.Should().HaveCount(1);
        }

        [Test]
        public virtual void Should_commit_literal_node()
        {
            // given

            // when
            IAgent entity=EntityContext.Create<IAgent>("http://magi/people/Tomasz");
            entity.Gender="male";
            EntityContext.Commit();

            // then
            EntityContext.Store.Quads.Should().HaveCount(2);
        }

        [Test]
        public virtual void Should_commit_blank_node()
        {
            // given

            // when
            IAgent entity=EntityContext.Create<IAgent>("http://magi/people/Tomasz");
            entity.KnowsOne=EntityContext.Create<IAgent>(entity.CreateBlankId());
            EntityContext.Commit();

            // then
            EntityContext.Store.Quads.Should().HaveCount(3);
        }

        [Test]
        public virtual void Should_remove_uri_node()
        {
            // given
            IAgent entity=EntityContext.Create<IAgent>("http://magi/people/Tomasz");
            EntityContext.Commit();

            // when
            EntityContext.Delete(entity.Id);
            EntityContext.Commit();

            // then
            EntityContext.Store.Quads.Should().HaveCount(0);
        }

        [Test]
        public virtual void Should_remove_literal_node()
        {
            // given
            IAgent entity=EntityContext.Create<IAgent>("http://magi/people/Tomasz");
            entity.Gender="male";
            EntityContext.Commit();

            // when
            entity.Gender=null;
            EntityContext.Commit();

            // then
            EntityContext.Store.Quads.Should().HaveCount(1);
        }

        [Test]
        public virtual void Should_remove_blank_node()
        {
            // given
            IAgent entity=EntityContext.Create<IAgent>("http://magi/people/Tomasz");
            entity.KnowsOne=EntityContext.Create<IAgent>(entity.CreateBlankId());
            EntityContext.Commit();

            // when
            EntityContext.Delete(entity.KnowsOne.Id);
            EntityContext.Commit();

            // then
            EntityContext.Store.Quads.Should().HaveCount(1);
        }

        [Test]
        public virtual void Should_remove_whole_entity_graph()
        {
            // given
            IAgent entity=EntityContext.Create<IAgent>("http://magi/people/Tomasz");
            entity.KnowsOne=EntityContext.Create<IAgent>(entity.CreateBlankId());
            entity.Gender="male";
            IAgent someEntity=EntityContext.Create<IAgent>("http://magi/people/Karol");
            someEntity.KnowsOne=entity;
            EntityContext.Commit();

            // when
            EntityContext.Delete(entity.Id,DeleteBehaviours.DeleteChildren|DeleteBehaviours.NullifyChildren);
            EntityContext.Commit();

            // then
            EntityContext.Store.Quads.Should().HaveCount(1);
        }

        [Test]
        public virtual void Should_reconstruct_entity()
        {
            // given
            IAlsoAgent entity=EntityContext.Create<IAlsoAgent>("http://magi/people/Tomasz");
            IAgent someAgent=EntityContext.Create<IAgent>(entity.CreateBlankId());
            IAgent anotherAgent=EntityContext.Create<IAgent>(entity.CreateBlankId());
            entity.Knows.Add(someAgent);
            entity.Knows.Add(anotherAgent);
            EntityContext.Commit();

            // when
            EntityContext.Delete(someAgent.Id,DeleteBehaviours.DeleteChildren|DeleteBehaviours.NullifyChildren);
            EntityContext.Commit();

            // then
            EntityContext.Store.Quads.Should().HaveCount(6);
        }

        protected override void ChildSetup()
        {
            Factory.WithNamedGraphSelector(new TestGraphSelector());
        }
    }
}