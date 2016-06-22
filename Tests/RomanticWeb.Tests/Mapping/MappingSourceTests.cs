using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RomanticWeb.Converters;
using RomanticWeb.Mapping.Model;
using RomanticWeb.TestEntities;
using RomanticWeb.TestEntities.Animals;
using RomanticWeb.TestEntities.Inheritance;

namespace RomanticWeb.Tests.Mapping
{
    [TestFixture]
    public abstract class MappingSourceTests : MappingRepositoryTestBase
    {
        [Test]
        public void Mapped_type_should_have_mapped_class()
        {
            // given
            var mapping = MappingsRepository.MappingFor<ICarnivore>();

            // when
            var classMappings = mapping.Classes.ToList();

            // then
            classMappings.Should().HaveCount(2);
            classMappings.Should()
                         .Contain(c => c.IsInherited && c.IsMatch(new[] { new Uri("http://example/livingThings#Animal") }));
            classMappings.Should()
                         .Contain(c => !c.IsInherited && c.IsMatch(new[] { new Uri("http://example/livingThings#Carnivore") }));
        }

        [Test]
        public void Mapped_type_should_contain_declared_properties()
        {
            // given
            var mapping = MappingsRepository.MappingFor<IAnimal>();

            // when
            var propertyMapping = mapping.PropertyFor("Name");

            // then
            Assert.That(propertyMapping.Name, Is.EqualTo("Name"));
            Assert.That(propertyMapping.Uri, Is.EqualTo(new Uri("http://example/livingThings#name")));
        }

        [Test]
        public void Type_can_be_mapped_without_rdf_type()
        {
            // given
            var mapping = MappingsRepository.MappingFor<IUntypedAnimal>();

            // then
            Assert.That(mapping.Classes, Is.Empty);
            Assert.That(mapping.PropertyFor("Name"), Is.Not.Null);
        }

        [Test]
        public void Mapped_type_should_contain_inherited_properties()
        {
            // given
            var mapping = MappingsRepository.MappingFor<ICarnivore>();

            // when
            var propertyMapping = mapping.PropertyFor("Name");

            // then
            Assert.That(propertyMapping.Name, Is.EqualTo("Name"));
            Assert.That(propertyMapping.Uri, Is.EqualTo(new Uri("http://example/livingThings#name")));
        }

        [Test]
        public void Subclass_should_inherit_parent_Class_mappings()
        {
            // given
            var mapping = MappingsRepository.MappingFor<IHuman>();

            // when
            var classMappings = mapping.Classes;

            // then
            classMappings.Should().HaveCount(4);
        }

        [Test]
        public void Subclass_should_inehrit_parent_generic_property_mapping()
        {
            // given
            var mapping = MappingsRepository.MappingFor<ISpecificContainer>();

            // when
            var itemsPropertyMapping = (ICollectionMapping)mapping.Properties.First(item => item.Name == "Items");

            // then
            itemsPropertyMapping.StoreAs.Should().Be(StoreAs.SimpleCollection);
        }

        [Test]
        public void Mapping_should_allow_dictionary_with_default_key_value()
        {
            // given
            var mapping = MappingsRepository.MappingFor<IEntityWithDictionary>();

            // then
            var propertyMapping = mapping.PropertyFor("SettingsDefault");
            propertyMapping.Should().BeAssignableTo<IDictionaryMapping>();
            propertyMapping.As<IDictionaryMapping>().KeyPredicate.Should().Be(Vocabularies.Rdf.predicate);
            propertyMapping.As<IDictionaryMapping>().ValuePredicate.Should().Be(Vocabularies.Rdf.@object);
        }

        [TestCase("DefaultListMapping", StoreAs.RdfList)]
        [TestCase("DefaultEnumerableMapping", StoreAs.SimpleCollection)]
        [TestCase("DefaultCollectionMapping", StoreAs.SimpleCollection)]
        public void Default_setting_for_collection_should_be_replaced_by_conventions(
            string propertyName, StoreAs asExpected)
        {
            // given
            var mapping = MappingsRepository.MappingFor<IEntityWithCollections>();

            // when
            var property = (ICollectionMapping)mapping.PropertyFor(propertyName);

            // then
            property.StoreAs.Should().Be(asExpected);
        }

        [TestCase("OverridenListMapping", StoreAs.SimpleCollection)]
        [TestCase("OverridenEnumerableMapping", StoreAs.RdfList)]
        [TestCase("OverridenCollectionMapping", StoreAs.RdfList)]
        public void Explicit_setting_for_collection_should_not_be_replaced_by_conventions(
            string propertyName, StoreAs asExpected)
        {
            // given
            var mapping = MappingsRepository.MappingFor<IEntityWithCollections>();

            // when
            var property = (ICollectionMapping)mapping.PropertyFor(propertyName);

            // then
            property.StoreAs.Should().Be(asExpected);
        }

        [Test]
        public void Explicit_setting_for_property_converter_should_not_be_replaced_by_convention()
        {
            // given
            var mapping = MappingsRepository.MappingFor<IEntityWithExplicitConverters>();

            // when
            var property = mapping.PropertyFor("Property");

            // then
            property.Converter.Should().BeOfType<BooleanConverter>();
        }

        [Test]
        public void Explicit_setting_for_collection_converter_should_not_be_replaced_by_convention()
        {
            // given
            var mapping = MappingsRepository.MappingFor<IEntityWithExplicitConverters>();

            // when
            var property = (ICollectionMapping)mapping.PropertyFor("Collection");

            // then
            property.ElementConverter.Should().BeOfType<BooleanConverter>();
        }

        [Test]
        public void Explicit_setting_for_dictionary_key_converter_should_not_be_replaced_by_convention()
        {
            // given
            var generatedEntityType = Type.GetType("RomanticWeb.TestEntities.IEntityWithExplicitConverters_Dictionary_Entry, RomanticWeb.TestEntities"); 
            var mapping = MappingsRepository.MappingFor(generatedEntityType);

            // when
            var property = mapping.PropertyFor("Key");

            // then
            property.Converter.Should().BeOfType<BooleanConverter>();
        }

        [Test]
        public void Explicit_setting_for_dictionary_value_converter_should_not_be_replaced_by_convention()
        {
            // given
            var generatedEntityType = Type.GetType("RomanticWeb.TestEntities.IEntityWithExplicitConverters_Dictionary_Entry, RomanticWeb.TestEntities");
            var mapping = MappingsRepository.MappingFor(generatedEntityType);

            // when
            var property = mapping.PropertyFor("Value");

            // then
            property.Converter.Should().BeOfType<BooleanConverter>();
        }

        [Test]
        public void Multimapping_should_not_throw_when_getting_derived_properties()
        {
            // given
            IEntityMapping herbivoreMapping = MappingsRepository.MappingFor<IHerbivore>();
            IEntityMapping carnivoreMapping = MappingsRepository.MappingFor<ICarnivore>();

            // when
            var multiMapping = new MultiMapping(herbivoreMapping, carnivoreMapping);

            // then
            Assert.DoesNotThrow(() => multiMapping.PropertyFor("Name"));
        }

        [Test]
        public void Should_provide_all_entity_mappings()
        {
            var mappings = MappingsRepository.Count();

            mappings.Should().NotBe(0);
        }
    }
}