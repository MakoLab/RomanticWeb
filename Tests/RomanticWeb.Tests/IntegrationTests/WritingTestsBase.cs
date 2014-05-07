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
        public void Should_commit_uri_node()
        {
            // given

            // when
            IAgent entity=EntityContext.Create<IAgent>("http://magi/people/Tomasz");
            EntityContext.Commit();

            // then
            EntityContext.Store.Quads.Should().HaveCount(1);
        }

        [Test]
        public void Should_commit_literal_node()
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
        public void Should_commit_blank_node()
        {
            // given

            // when
            IAgent entity=EntityContext.Create<IAgent>("http://magi/people/Tomasz");
            entity.KnowsOne=EntityContext.Create<IAgent>(entity.CreateBlankId());
            EntityContext.Commit();

            // then
            EntityContext.Store.Quads.Should().HaveCount(3);
        }

        protected override void ChildSetup()
        {
            Factory.WithNamedGraphSelector(new TestGraphSelector());
        }
    }
}