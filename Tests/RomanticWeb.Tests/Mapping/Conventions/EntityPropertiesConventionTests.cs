using System;
using System.Collections.Generic;
using FluentAssertions;
using ImpromptuInterface;
using NUnit.Framework;
using RomanticWeb.Converters;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Conventions;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.TestEntities;
using RomanticWeb.Tests.Stubs;

namespace RomanticWeb.Tests.Mapping.Conventions
{
    [TestFixture]
    public class EntityPropertiesConventionTests
    {
        private EntityPropertiesConvention _convention;

        private interface ITestEntity : IEntity
        {
        }

        [SetUp]
        public void Setup()
        {
            _convention = new EntityPropertiesConvention();
        }

        [TestCase(typeof(ITestEntity))]
        [TestCase(typeof(IList<ITestEntity>), Description = "Should work for collection type")]
        [TestCase(typeof(IEntity))]
        [TestCase(typeof(IList<IEntity>), Description = "Should work for collection type")]
        public void Should_be_applied_to_property_IEntity_property_type(Type type)
        {
            // given
            var convention = new
            {
                ConverterType = default(Type),
                PropertyInfo = new TestPropertyInfo(type)
            }.ActLike<IPropertyMappingProvider>();

            // when
            var shouldApply = _convention.ShouldApply(convention);

            // then
            shouldApply.Should().BeTrue();
        }

        [TestCase(typeof(ITestEntity), typeof(AsEntityConverter<ITestEntity>))]
        [TestCase(typeof(IList<ITestEntity>), typeof(AsEntityConverter<ITestEntity>), Description = "Should work for collection type")]
        [TestCase(typeof(IPerson), typeof(AsEntityConverter<IPerson>))]
        [TestCase(typeof(IList<IEntity>), typeof(AsEntityConverter<ITestEntity>), Description = "Should work for collection type")]
        public void Should_set_converter_for_property_with_IEntity_property_type(Type type, Type converterType)
        {
            // given
            var convention = new
            {
                ConverterType = default(Type),
                PropertyInfo = new TestPropertyInfo(type)
            }.ActLike<IPropertyMappingProvider>();

            // when
            var shouldApply = _convention.ShouldApply(convention);

            // then
            shouldApply.Should().BeTrue();
        }
    }
}