using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Sources;
using RomanticWeb.TestEntities.MixedMappings;
using RomanticWeb.TestEntities.MixedMappings.AmbiguousMembers;

namespace RomanticWeb.Tests.Mapping
{
    [TestFixture]
    public class MixedMappingTests : MappingRepositoryTestBase
    {
        [Test]
        public void Should_use_open_generic_mapping_fallback()
        {
            // given
            var mapping = MappingsRepository.MappingFor<IDerived>();

            // when
            var property = mapping.PropertyFor("MappedProperty2");

            // then
            property.Uri.Should().Be(new Uri("urn:open:mapping2"));
        }

        [Test]
        public void Should_fill_return_types_for_inherited_generic_properties()
        {
            // given
            var mapping = MappingsRepository.MappingFor<IDerived>();

            // when
            var property = mapping.PropertyFor("MappedProperty2");

            // then
            property.ReturnType.Should().Be(typeof(int));
        }

        [Test]
        public void Should_contain_closed_generic_mappings_not_mapped_explicitly()
        {
            // given
            var mapping = MappingsRepository.MappingFor<IGenericParent<int>>();

            // when
            var property = mapping.PropertyFor("MappedProperty2");

            // then
            property.ReturnType.Should().Be(typeof(int));
        }

        [Test]
        public void Should_contain_open_generic_mappings()
        {
            // given
            var mapping = MappingsRepository.MappingFor(typeof(IGenericParent<>));

            // when
            var property = mapping.PropertyFor("MappedProperty2");

            // then
            property.ReturnType.Name.Should().Be("T");
        }

        [Test]
        public void Should_respect_hiding_members()
        {
            // given, when
            var mapping = MappingsRepository.MappingFor<IHidesMember>();

            // then
            mapping.PropertyFor("MappedProperty1").Uri.Should().Be(new Uri("urn:hidden:mapping"));
            mapping.PropertyFor("MappedProperty2").Uri.Should().Be(new Uri("urn:hidden:fluent"));
        }

        [TestCase(typeof(IDerived), "urn:override:fluent1")]
        [TestCase(typeof(IDerivedLevel2), "urn:override:fluent2")]
        public void Should_respect_mapping_override(Type type, string expectedUri)
        {
            // given
            var mapping = MappingsRepository.MappingFor(type);

            // when
            var property = mapping.PropertyFor("MappedProperty1");

            // then
            property.Uri.Should().Be(new Uri(expectedUri));
        }

        [Test]
        public void Should_retain_class_inheritance_in_concrete_entities()
        {
            // given
            var mapping = MappingsRepository.MappingFor<ConcreteEntityChild>();

            // when
            var property = mapping.PropertyFor("UnMappedProperty");

            // then
            property.Uri.Should().Be(new Uri("urn:concrete:class"));
        }

        [Test]
        public void Should_inherit_properties_in_concrete_entity_class()
        {
            // given
            var mapping = MappingsRepository.MappingFor<ConcreteEntity>();

            // when
            var property = mapping.PropertyFor("MappedProperty1");

            // then
            property.Uri.Should().Be(new Uri("urn:open:mapping1"));
        }

        [Test]
        public void Should_read_all_properties_from_fluent_mapped_hierarchy_of_concrete_classes()
        {
            // given
            var mapping = MappingsRepository.MappingFor<FluentNoIEntityInnerMapChild>();

            // when
            var properties = mapping.Properties;

            // then
            properties.Should().HaveCount(2);
        }

        [Test]
        public void Should_fail_when_getting_ambiguous_property_mapping()
        {
            // given
            var mapping = MappingsRepository.MappingFor<IChild>();
            
            // then
            mapping.Invoking(m => m.PropertyFor("Member")).ShouldThrow<AmbiguousPropertyException>();
        }

        [Test]
        public void Should_give_access_to_all_ambiguous_property_mappings()
        {
            // given
            var mapping = MappingsRepository.MappingFor<IChild>();

            // when
            var properties = mapping.Properties;

            // then
            properties.Should().HaveCount(4);
        }

        [Test]
        public void Should_provide_all_entity_mappings()
        {
            var mappings = MappingsRepository.Count();

            mappings.Should().NotBe(0);
        }

        protected override IEnumerable<IMappingProviderSource> CreateMappingSources()
        {
            yield return new FluentMappingsSource(typeof(IHidesMember).Assembly);
            yield return new AttributeMappingsSource(typeof(IHidesMember).Assembly);
        }
    }
}