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
    public class RdfListConventionTests
    {
        private static readonly dynamic New = Builder.New();
        private RdfListConvention _rdfListConvention;

        [SetUp]
        public void Setup()
        {
            _rdfListConvention = new RdfListConvention();
        }

        [Test]
        public void Should_be_applied_for_IList()
        {
            // given
            var mapping = new
            {
                PropertyInfo=(PropertyInfo)new TestPropertyInfo(typeof(IList<int>)),
                StoreAs=StoreAs.Undefined
            }.ActLike<ICollectionMappingProvider>();

            // when
            var shouldApply=_rdfListConvention.ShouldApply(mapping);

            // then
            shouldApply.Should().BeTrue();
        }

        [TestCase(typeof(ICollection<int>))]
        [TestCase(typeof(IEnumerable<int>))]
        [TestCase(typeof(ISet<int>))]
        [TestCase(typeof(IDictionary<int, string>))]
        public void Should_not_be_applied_for_non_list_collections(Type collectionType)
        {
            // given
            var mapping = new
            {
                PropertyInfo=(PropertyInfo)new TestPropertyInfo(collectionType),
                StoreAs=StoreAs.Undefined
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
                PropertyInfo=(PropertyInfo)new TestPropertyInfo(typeof(int)),
                StoreAs=StoreAs.Undefined
            }.ActLike<ICollectionMappingProvider>();

            // when
            var shouldApply=_rdfListConvention.ShouldApply(mapping);

            // then
            shouldApply.Should().BeFalse();
        }

        [Test]
        public void Applying_should_set_StorageStrategy()
        {
            // given
            ICollectionMappingProvider mapping = New.ExpandoObject(
                PropertyInfo: (PropertyInfo)new TestPropertyInfo(typeof(int)),
                StoreAs: StoreAs.Undefined).ActLike<ICollectionMappingProvider>();

            // when
            _rdfListConvention.Apply(mapping);

            // then
            mapping.StoreAs.Should().Be(StoreAs.RdfList);
        }
    }
}   