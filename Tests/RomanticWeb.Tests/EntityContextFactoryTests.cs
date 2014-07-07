using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RomanticWeb.Mapping;
using RomanticWeb.Ontologies;
using RomanticWeb.TestEntities;

namespace RomanticWeb.Tests
{
    [TestFixture]
    public class EntityContextFactoryTests
    {
        private EntityContextFactory _entityContextFactory;
        private Mock<IOntologyProvider> _ontology;

        [SetUp]
        public void Setup()
        {
            _ontology = new Mock<IOntologyProvider>();
            _ontology.Setup(provider => provider.ResolveUri(It.IsAny<string>(), It.IsAny<string>())).Returns((string prefix, string name) => new Uri(new Uri("http://base/"), name));
            _entityContextFactory = new EntityContextFactory().WithOntology(_ontology.Object);
        }

        [TearDown]
        public void Teardown()
        {
        }

        [Test]
        public void Adding_attribute_mappings_for_an_Assembly_twice_should_add_only_one_repository_single_call()
        {
            // given
            var withMappings = typeof(IPerson).Assembly;

            // when
            _entityContextFactory.WithMappings(
                m =>
                {
                    m.Attributes.FromAssembly(withMappings);
                    m.Attributes.FromAssemblyOf<IPerson>();
                });

            // then
            _entityContextFactory.Mappings.Should().BeOfType<MappingsRepository>();
            _entityContextFactory.Mappings.As<MappingsRepository>().Sources.Should().HaveCount(3);
        }

        [Test]
        public void Adding_fluent_mappings_for_an_Assembly_twice_should_add_only_one_repository_single_call()
        {
            // given
            var withMappings = typeof(IPerson).Assembly;

            // when
            _entityContextFactory.WithMappings(
                m =>
                {
                    m.Fluent.FromAssembly(withMappings);
                    m.Fluent.FromAssemblyOf<IPerson>();
                });

            // then
            _entityContextFactory.Mappings.As<MappingsRepository>().Sources.Should().HaveCount(3);
        }

        [Test, Description("Calling WithMappings twice")]
        public void Adding_attribute_mappings_for_an_Assembly_twice_should_add_only_one_repository()
        {
            // given
            var withMappings = typeof(IPerson).Assembly;

            // when
            _entityContextFactory.WithMappings(m => m.Attributes.FromAssembly(withMappings));
            _entityContextFactory.WithMappings(m => m.Attributes.FromAssemblyOf<IPerson>());

            // then
            _entityContextFactory.Mappings.Should().BeOfType<MappingsRepository>();
            _entityContextFactory.Mappings.As<MappingsRepository>().Sources.Should().HaveCount(3);
        }

        [Test, Description("Calling WithMappings twice")]
        public void Adding_fluent_mappings_for_an_Assembly_twice_should_add_only_one_repository()
        {
            // given
            var withMappings = typeof(IPerson).Assembly;

            // when
            _entityContextFactory.WithMappings(m => m.Fluent.FromAssembly(withMappings));
            _entityContextFactory.WithMappings(m => m.Fluent.FromAssemblyOf<IPerson>());

            // then
            _entityContextFactory.Mappings.Should().BeOfType<MappingsRepository>();
            _entityContextFactory.Mappings.As<MappingsRepository>().Sources.Should().HaveCount(3);
        }
    }
}