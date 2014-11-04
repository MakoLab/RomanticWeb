using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RomanticWeb.TestEntities.Generic;
using RomanticWeb.TestEntities.Inheritance;

namespace RomanticWeb.Tests
{
    [TestFixture]
    public class TypeComparerTests
    {
        private IComparer<Type> _typeComparer;

        private interface ILocalSpecificContainer : IGenericContainer<IInterface>
        {
        }

        [SetUp]
        public void Setup()
        {
            _typeComparer = TypeComparer.Default;
        }

        [Test]
        public void Open_generic_should_be_greater_than_closed_generic()
        {
            // given
            var open = typeof(IGenericEntityWithAnyArgument<>);
            var closed = typeof(IGenericEntityWithAnyArgument<int>);

            // then
            _typeComparer.Compare(open, closed).Should().BeGreaterThan(0);
        }

        [Test]
        public void Open_generic_should_be_less_than_closed_generic()
        {
            // given
            var open = typeof(IGenericEntityWithAnyArgument<>);
            var closed = typeof(IGenericEntityWithAnyArgument<int>);

            // then
            _typeComparer.Compare(closed, open).Should().BeLessThan(0);
        }

        [Test]
        public void Should_order_types_correctly()
        {
            // given
            var baseClass = typeof(IGenericContainer);
            var open = typeof(IGenericContainer<>);
            var closed = typeof(IGenericContainer<IInterface>);
            var derived = typeof(ISpecificContainer);
            var list = new[] { open, derived, closed, baseClass };

            // when
            var ordered = list.OrderByDescending(t => t, _typeComparer);

            // then
            ordered.Should().ContainInOrder(new object[] { baseClass, open, closed, derived });
        }

        [Test]
        public void Should_order_correctly_types_from_various_assemblies()
        {
            // given
            var baseClass = typeof(IGenericContainer);
            var open = typeof(IGenericContainer<>);
            var closed = typeof(IGenericContainer<IInterface>);
            var derived = typeof(ILocalSpecificContainer);
            var list = new[] { open, derived, closed, baseClass };

            // when
            var ordered = list.OrderByDescending(t => t, _typeComparer);

            // then
            ordered.Should().ContainInOrder(new object[] { baseClass, open, closed, derived });
        }
    }
}