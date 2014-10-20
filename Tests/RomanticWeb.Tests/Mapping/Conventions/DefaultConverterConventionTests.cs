using System;
using System.Collections.Generic;
using FluentAssertions;
using ImpromptuInterface;
using ImpromptuInterface.Dynamic;
using NUnit.Framework;
using RomanticWeb.Converters;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Conventions;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.Tests.Stubs;

namespace RomanticWeb.Tests.Mapping.Conventions
{
    [TestFixture]
    public class DefaultConverterConventionTests
    {
        private static readonly dynamic New = Builder.New();
        private DefaultConvertersConvention _convention;

        [SetUp]
        public void Setup()
        {
            _convention = new DefaultConvertersConvention();
        }

        [TearDown]
        public void Teardown()
        {
        }

        [TestCase(typeof(int))]
        [TestCase(typeof(IList<int>), Description = "Should work for collection type")]
        [TestCase(typeof(int?), Description = "Should work for nullable type")]
        public void Should_be_applied_to_property_without_converter_with_known_property_type(Type type)
        {
            // given
            _convention.SetDefault<int, IntegerConverter>();
            var mapping = new
                               {
                                   ConverterType = default(Type),
                                   PropertyInfo = new TestPropertyInfo(type)
                               }.ActLike<IPropertyMappingProvider>();

            // when
            var shouldApply = ((IPropertyConvention)_convention).ShouldApply(mapping);

            // then
            shouldApply.Should().BeTrue();
        }

        [Test]
        public void Should_not_be_applied_to_property_with_unknown_property_type()
        {
            // given
            var mapping = new
                               {
                                   ConverterType = default(Type),
                                   PropertyInfo = new TestPropertyInfo(typeof(float))
                               }.ActLike<IPropertyMappingProvider>();

            // when
            var shouldApply = ((IPropertyConvention)_convention).ShouldApply(mapping);

            // then
            shouldApply.Should().BeFalse();
        }

        [Test]
        public void Applying_should_set_default_converter_type_for_known_property_type()
        {
            // given
            _convention.SetDefault<int, IntegerConverter>();
            IPropertyMappingProvider mapping = New.ExpandoObject(
                PropertyInfo: new TestPropertyInfo(typeof(int)),
                ConverterType: default(Type)).ActLike<ICollectionMappingProvider>();

            // when
            ((IPropertyConvention)_convention).Apply(mapping);

            // then
            mapping.ConverterType.Should().Be(typeof(IntegerConverter));
        }

        [Test]
        public void Applying_should_set_default_converter_for_simple_collection()
        {
            // given
            _convention.SetDefault<int, IntegerConverter>();
            IPropertyMappingProvider mapping = New.ExpandoObject(
                PropertyInfo: new TestPropertyInfo(typeof(IList<int>)),
                ConverterType: default(Type)).ActLike<ICollectionMappingProvider>();

            // when
            ((IPropertyConvention)_convention).Apply(mapping);

            // then
            mapping.ConverterType.Should().Be(typeof(IntegerConverter));
        }

        [Test]
        public void Applying_should_not_set_default_converter_for_rdf_list()
        {
            // given
            _convention.SetDefault<int, IntegerConverter>();
            IPropertyMappingProvider mapping = New.ExpandoObject(
                PropertyInfo: new TestPropertyInfo(typeof(IList<int>)),
                ConverterType: default(Type),
                StoreAs: StoreAs.RdfList).ActLike<ICollectionMappingProvider>();

            // when
            ((IPropertyConvention)_convention).Apply(mapping);

            // then
            mapping.ConverterType.Should().Be(typeof(AsEntityConverter<IEntity>));
        }

        [Test]
        public void Applying_should_set_default_converter_for_collection_elements()
        {
            // given
            _convention.SetDefault<int, IntegerConverter>();
            ICollectionMappingProvider mapping = New.ExpandoObject(
                PropertyInfo: new TestPropertyInfo(typeof(IList<int>)),
                ConverterType: default(Type),
                ElementConverterType: typeof(Type)).ActLike<ICollectionMappingProvider>();

            // when
            ((ICollectionConvention)_convention).Apply(mapping);

            // then
            mapping.ElementConverterType.Should().Be(typeof(IntegerConverter));
        }

        [Test]
        public void Should_apply_set_converter_for_explicitly_defined_array_type()
        {
            // given
            _convention.SetDefault<int[], Base64BinaryConverter>();
            _convention.SetDefault<int, IntegerConverter>();
            ICollectionMappingProvider mapping = New.ExpandoObject(
                PropertyInfo: new TestPropertyInfo(typeof(int[])),
                ConverterType: default(Type)).ActLike<ICollectionMappingProvider>();

            // when
            ((ICollectionConvention)_convention).Apply(mapping);

            // then
            mapping.ConverterType.Should().Be(typeof(Base64BinaryConverter));
        }

        [Test]
        public void Should_apply_converter_to_dictionary_key()
        {
            // given
            _convention.SetDefault<int, IntegerConverter>();
            IDictionaryMappingProvider mapping = New.ExpandoObject(
                PropertyInfo: new TestPropertyInfo(typeof(IList<int>)),
                ConverterType: default(Type),
                ElementConverterType: typeof(Type)).ActLike<IDictionaryMappingProvider>();

            // when
            ((IDictionaryConvention)_convention).Apply(mapping);

            // then
            mapping.Key.ConverterType.Should().Be(typeof(IntegerConverter));
        }

        [Test]
        public void Should_apply_converter_to_dictionary_element()
        {
            // given
            _convention.SetDefault<int, IntegerConverter>();
            IDictionaryMappingProvider mapping =
                New.ExpandoObject(
                    PropertyInfo: new TestPropertyInfo(typeof(IList<int>)),
                    ConverterType: default(Type),
                    ElementConverterType: typeof(Type)).ActLike<IDictionaryMappingProvider>();

            // when
            ((IDictionaryConvention)_convention).Apply(mapping);

            // then
            mapping.Key.ConverterType.Should().Be(typeof(IntegerConverter));
        }
    }
}