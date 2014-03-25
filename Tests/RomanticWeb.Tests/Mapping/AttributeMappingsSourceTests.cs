using System;
using System.Collections.Generic;
using NUnit.Framework;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.TestEntities.Animals;
using RomanticWeb.TestEntities.Generic;

namespace RomanticWeb.Tests.Mapping
{
    [TestFixture]
    public class AttributeMappingsSourceTests:MappingSourceTests
    {
        [Test]
        public void Mapping_of_class_derived_from_generic_class_should_contain_parent_properties_mapped_in_open_generic()
        {
            // given
            var mapping=MappingsRepository.MappingFor<IImplementsGenericWithInt>();

            // when
            var propertyMapping=mapping.PropertyFor("Property");

            // then
            Assert.That(propertyMapping.Name,Is.EqualTo("Property"));
            Assert.That(propertyMapping.Uri,Is.EqualTo(new Uri("urn:generic:property")));
        }

        [Test]
        public void Mapping_of_class_derived_from_generic_class_should_contain_parent_collections_mapped_in_open_generic()
        {
            // given
            var mapping=MappingsRepository.MappingFor<IImplementsGenericWithInt>();

            // when
            var propertyMapping=mapping.PropertyFor("Collection");

            // then
            Assert.That(propertyMapping.Name,Is.EqualTo("Collection"));
            Assert.That(propertyMapping.Uri,Is.EqualTo(new Uri("urn:generic:collection")));
        }

        protected override IEnumerable<IMappingProviderSource> CreateMappingSources()
        {
            yield return new AttributeMappingsSource(typeof(IAnimal).Assembly);
        }
    }
}