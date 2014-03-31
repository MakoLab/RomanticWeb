using System;
using System.Collections.Generic;
using FluentAssertions;
using ImpromptuInterface;
using ImpromptuInterface.Dynamic;
using NUnit.Framework;
using RomanticWeb.Converters;
using RomanticWeb.Mapping.Conventions;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.Tests.Stubs;

namespace RomanticWeb.Tests.Mapping.Conventions
{
    [TestFixture]
    public class DefaultConverterConventionTests
    {
        private static readonly dynamic New=Builder.New();
        private DefaultConvertersConvention _convention;

        [SetUp]
        public void Setup()
        {
            _convention=new DefaultConvertersConvention();
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
            _convention.SetDefault<int,IntegerConverter>();
            var convention=new
                               {
                                   ConverterType=default(Type),
                                   PropertyInfo=new TestPropertyInfo(type)
                               }.ActLike<IPropertyMappingProvider>();

            // when
            var shouldApply=_convention.ShouldApply(convention);

            // then
            shouldApply.Should().BeTrue();
        }

        [Test]
        public void Should_not_be_applied_to_property_with_unknown_property_type()
        {
            // given
            var convention=new
                               {
                                   ConverterType=default(Type),
                                   PropertyInfo=new TestPropertyInfo(typeof(float))
                               }.ActLike<IPropertyMappingProvider>();

            // when
            var shouldApply=_convention.ShouldApply(convention);

            // then
            shouldApply.Should().BeFalse();
        }

        [TestCase(typeof(int))]
        [TestCase(typeof(IList<int>),Description="Should work for collection type")]
        public void Applying_should_set_default_converter_type_for_known_property_type(Type type)
        {
            // given
            _convention.SetDefault<int,IntegerConverter>();
            IPropertyMappingProvider mapping = New.ExpandoObject(
                PropertyInfo: new TestPropertyInfo(type),
                ConverterType: default(Type)).ActLike<ICollectionMappingProvider>();

            // when
            _convention.Apply(mapping);

            // then
            mapping.ConverterType.Should().Be(typeof(IntegerConverter));
        }

        [Test]
        public void Should_apply_set_converter_for_explicitly_defined_array_type()
        {
            // given
            _convention.SetDefault<int[], Base64BinaryConverter>();
            _convention.SetDefault<int, IntegerConverter>();
            IPropertyMappingProvider mapping = New.ExpandoObject(
                PropertyInfo: new TestPropertyInfo(typeof(int[])),
                ConverterType: default(Type)).ActLike<ICollectionMappingProvider>();

            // when
            _convention.Apply(mapping);

            // then
            mapping.ConverterType.Should().Be(typeof(Base64BinaryConverter));
        }
    }
}