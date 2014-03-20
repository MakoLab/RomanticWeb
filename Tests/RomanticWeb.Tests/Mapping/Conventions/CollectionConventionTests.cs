using System;
using System.Collections.Generic;
using System.Reflection;
using FluentAssertions;
using ImpromptuInterface;
using ImpromptuInterface.Dynamic;
using NUnit.Framework;
using RomanticWeb.Mapping.Conventions;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.Tests.Stubs;

namespace RomanticWeb.Tests.Mapping.Conventions
{
    [TestFixture]
    public class CollectionConventionTests
    {
        private static readonly dynamic New=Builder.New();
        private CollectionConvention _rdfListConvention;

        [SetUp]
        public void Setup()
        {
            _rdfListConvention=new CollectionConvention();
        }

        [Test]
        public void Should_be_applied_for_ICollection()
        {
            // given
            var mapping = new
            {
                PropertyInfo = (PropertyInfo)new TestPropertyInfo(typeof(ICollection<int>)),
                StoreAs = default(StoreAs)
            }.ActLike<ICollectionMappingProvider>();

            // when
            var shouldApply = _rdfListConvention.ShouldApply(mapping);

            // then
            shouldApply.Should().BeTrue();
        }

        [Test]
        public void Should_be_applied_for_IEnumerable()
        {
            // given
            var mapping = new
            {
                PropertyInfo = (PropertyInfo)new TestPropertyInfo(typeof(IEnumerable<int>)),
                StoreAs = default(StoreAs)
            }.ActLike<ICollectionMappingProvider>();

            // when
            var shouldApply = _rdfListConvention.ShouldApply(mapping);

            // then
            shouldApply.Should().BeTrue();
        }

        [TestCase(typeof(IList<int>))]
        [TestCase(typeof(ISet<int>))]
        [TestCase(typeof(IDictionary<int, string>))]
        public void Should_be_applied_for_collections(Type collectionType)
        {
            // given
            var mapping = new
            {
                PropertyInfo = (PropertyInfo)new TestPropertyInfo(collectionType),
                StoreAs = default(StoreAs)
            }.ActLike<ICollectionMappingProvider>();

            // when
            var shouldApply=_rdfListConvention.ShouldApply(mapping);

            // then
            shouldApply.Should().BeFalse();
        }

        [Test]
        public void Should_not_be_applied_for_non_collections()
        {
            // given
            var mapping = new
            {
                PropertyInfo = (PropertyInfo)new TestPropertyInfo(typeof(int)),
                StoreAs = default(StoreAs)
            }.ActLike<ICollectionMappingProvider>();

            // when
            var shouldApply = _rdfListConvention.ShouldApply(mapping);

            // then
            shouldApply.Should().BeFalse();
        }

        [Test]
        public void Applying_should_set_StorageStrategy()
        {
            // given
            ICollectionMappingProvider mapping = New.ExpandoObject(
                ReturnType: (PropertyInfo)new TestPropertyInfo(typeof(int)),
                StoreAs: StoreAs.Undefined).ActLike<ICollectionMappingProvider>();

            // when
            _rdfListConvention.Apply(mapping);

            // then
            mapping.StoreAs.Should().Be(StoreAs.SimpleCollection);
        }
    }
}