using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RomanticWeb.TestEntities.MixedMappings;

namespace RomanticWeb.Tests
{
    [TestFixture]
    public class TypeExtensionsTests
    {
        [Test]
        public void GetImmediateParents_should_find_directly_implemented_interfaces()
        {
            // given
            Type type=typeof(IDerivedLevel2);

            // when
            var immediateParents=type.GetImmediateParents().ToList();

            // then
            immediateParents.Should().HaveCount(1);
            immediateParents.Should().Contain(typeof(IDerived));
        }

        [Test]
        public void GetImmediateParents_should_find_open_generic_type_for_cloes_generics()
        {
            // given
            Type type = typeof(IGenericParent<int>);

            // when
            var immediateParents = type.GetImmediateParents().ToList();

            // then
            immediateParents.Should().HaveCount(1);
            immediateParents.Should().Contain(typeof(IGenericParent<>));
        }

        [Test]
        public void GetImmediateParents_should_not_include_IEntity()
        {
            // given
            Type type = typeof(TestEntities.IEntityWithDictionary);

            // when
            var immediateParents = type.GetImmediateParents().ToList();

            // then
            immediateParents.Should().BeEmpty();
        }

        [Test]
        public void GetImmediateParents_should_include_concrete_base_type()
        {
            // given
            Type type=typeof(FluentNoIEntityInnerMapChild);

            // when
            var immediateParents=type.GetImmediateParents().ToList();

            // then
            immediateParents.Should().Contain(typeof(FluentNoIEntityInnerMapParent));
        }

        [Test]
        public void GetImmediateParents_of_open_generic_should_not_return_itself()
        {
            // given
            Type type=typeof(IGenericParent<>);

            // when
            var immediateParents=type.GetImmediateParents().ToList();

            // then
            immediateParents.Should().BeEmpty();
        }
    }
}