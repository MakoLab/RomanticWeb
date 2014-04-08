using FluentAssertions;
using NUnit.Framework;
using RomanticWeb.Entities;
using RomanticWeb.TestEntities.Foaf;
using RomanticWeb.Tests.Stubs;

namespace RomanticWeb.Tests.IntegrationTests
{
    public abstract class LoadingTestsBase:IntegrationTestsBase
    {
        private static readonly EntityId EntityId=new EntityId("http://magi/people/Tomasz");

        [Test]
        public void Should_load_as_best_derived_type_in_inheritance_tree()
        {
            // given
            LoadTestFile("InheritingEntities.trig");

            // when
            var entity=EntityContext.Load<IAgent>("http://magi/people/Tomasz");

            // then
            entity.Should().BeAssignableTo<IPerson>();
        }

        [Test]
        public void Should_load_entity_as_best_types()
        {
            // given
            LoadTestFile("InheritingEntities.trig");

            // when
            var entity=EntityContext.Load<IEntity>(EntityId);

            // then
            entity.Should().BeAssignableTo<IPerson>();
            entity.Should().BeAssignableTo<IAlsoPerson>();
        }

        [Test]
        public void Entity_should_be_castable_to_multiple_types()
        {
            // given
            LoadTestFile("InheritingEntities.trig");

            // when
            dynamic entity=EntityContext.Load<IEntity>(EntityId);

            // then
            Assert.That(entity is IPerson);
            Assert.That(entity is IAlsoPerson);
            ((IPerson)entity).Name.Should().Be("Tomasz");
            ((IAlsoPerson)entity).LastName.Should().Be("Pluskiewicz");
        }

        [Test]
        public void Should_load_entity_with_all_matching_derived_types()
        {
            // given
            LoadTestFile("EntityWithManyTypes.trig");

            // when
            var entity=EntityContext.Load<IAgent>(EntityId);

            // then
            entity.Should().BeAssignableTo<TestEntities.Foaf.IPerson>();
        }

        [Test]
        public void Should_load_associated_entities_with_all_matching_derived_types()
        {
            // given
            LoadTestFile("EntityWithManyTypes.trig");
            var entity=EntityContext.Load<IPerson>(EntityId);

            // when
            var karol=entity.KnowsOne;

            // then
            karol.Should().BeAssignableTo<TestEntities.Foaf.IPerson>();
        }

        [Test]
        public void Should_change_entity_type_best_matching_derived_type()
        {
            // given
            LoadTestFile("EntityWithManyTypes.trig");
            var entity=EntityContext.Load<ITypedEntity>(EntityId);

            // when
            var agent=entity.AsEntity<IAgent>();

            // then
            agent.Should().BeAssignableTo<TestEntities.Foaf.IPerson>();
        }

        protected override void ChildSetup()
        {
            Factory.WithNamedGraphSelector(new TestGraphSelector());
        }
    }
}