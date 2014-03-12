using System;
using System.Collections.Generic;
using FluentAssertions;
using ImpromptuInterface;
using ImpromptuInterface.Dynamic;
using NUnit.Framework;
using RomanticWeb.Entities.ResultAggregations;
using RomanticWeb.Mapping.Conventions;
using RomanticWeb.Mapping.Model;

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
                ReturnType = typeof(ICollection<int>),
                Aggregation = default(Aggregation?)
            }.ActLike<ICollectionMapping>();

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
                ReturnType = typeof(IEnumerable<int>),
                Aggregation = default(Aggregation?)
            }.ActLike<ICollectionMapping>();

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
                                  ReturnType=collectionType,
                                  Aggregation=default(Aggregation?)
                              }.ActLike<ICollectionMapping>();

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
                                  ReturnType = typeof(int),
                                  Aggregation = default(Aggregation?)
                              }.ActLike<ICollectionMapping>();

            // when
            var shouldApply = _rdfListConvention.ShouldApply(mapping);

            // then
            shouldApply.Should().BeFalse();
        }

        [Test]
        public void Applying_should_set_aggregation()
        {
            // given
            ICollectionMapping mapping = New.ExpandoObject(
                ReturnType: typeof(int),
                Aggregation: Aggregation.Has).ActLike<ICollectionMapping>();

            // when
            _rdfListConvention.Apply(mapping);

            // then
            mapping.Aggregation.Should().Be(Aggregation.Original);
        }
    }
}