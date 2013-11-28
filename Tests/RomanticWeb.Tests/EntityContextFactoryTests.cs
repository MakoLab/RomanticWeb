using FluentAssertions;
using NUnit.Framework;
using RomanticWeb.TestEntities;

namespace RomanticWeb.Tests
{
    [TestFixture]
    public class EntityContextFactoryTests
    {
        private EntityContextFactory _entityContextFactory;

        [SetUp]
        public void Setup()
        {
            _entityContextFactory=new EntityContextFactory();
        }

        [TearDown]
        public void Teardown()
        {
        }

        [Test]
        public void Adding_attribute_mappings_for_an_Assembly_twice_should_add_only_one_repository()
        {
            // given
            var withMappings=typeof(IPerson).Assembly;

            // when
            _entityContextFactory.Mappings(
                m =>
                    {
                        m.Attributes.FromAssembly(withMappings);
                        m.Attributes.FromAssemblyOf<IPerson>();
                    });

            // then
            _entityContextFactory.MappingRepositories.Should().HaveCount(1);
        }

        [Test]
        public void Adding_fluent_mappings_for_an_Assembly_twice_should_add_only_one_repository()
        {
            // given
            var withMappings = typeof(IPerson).Assembly;

            // when
            _entityContextFactory.Mappings(
                m =>
                {
                    m.Fluent.FromAssembly(withMappings);
                    m.Fluent.FromAssemblyOf<IPerson>();
                });

            // then
            _entityContextFactory.MappingRepositories.Should().HaveCount(1);
        }
    }
}