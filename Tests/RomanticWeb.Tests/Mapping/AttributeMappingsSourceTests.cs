using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RomanticWeb.Mapping.Sources;
using RomanticWeb.TestEntities.Animals;
using RomanticWeb.TestEntities.Generic;

namespace RomanticWeb.Tests.Mapping
{
    [TestFixture]
    public class AttributeMappingsSourceTests : MappingSourceTests
    {
        [Test]
        public void Mapping_of_class_derived_from_generic_class_should_contain_parent_properties_mapped_in_open_generic()
        {
            // given
            var mapping = MappingsRepository.MappingFor<IImplementsGenericWithInt>();

            // when
            var propertyMapping = mapping.PropertyFor("Property");

            // then
            propertyMapping.Name.Should().Be("Property");
            propertyMapping.Uri.Should().Be(new Uri("urn:generic:property"));
        }

        [Test]
        public void Mapping_of_class_derived_from_generic_class_should_contain_parent_collections_mapped_in_open_generic()
        {
            // given
            var mapping = MappingsRepository.MappingFor<IImplementsGenericWithInt>();

            // when
            var propertyMapping = mapping.PropertyFor("Collection");

            // then
            propertyMapping.Name.Should().Be("Collection");
            propertyMapping.Uri.Should().Be(new Uri("urn:generic:collection"));
        }

        [Test]
        public void Should_provide_all_entity_mappings()
        {
            var mappings = MappingsRepository.Count();

            mappings.Should().NotBe(0);
        }

        protected override IEnumerable<IMappingProviderSource> CreateMappingSources()
        {
            yield return new AttributeMappingsSource(typeof(IAnimal).Assembly);
        }
    }
}