using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RomanticWeb.Entities;
using RomanticWeb.TestEntities.Foaf;
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
            Type type = typeof(IDerivedLevel2);

            // when
            var immediateParents = type.GetImmediateParents().ToList();

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
            Type type = typeof(FluentNoIEntityInnerMapChild);

            // when
            var immediateParents = type.GetImmediateParents().ToList();

            // then
            immediateParents.Should().Contain(typeof(FluentNoIEntityInnerMapParent));
        }

        [Test]
        public void GetImmediateParents_of_open_generic_should_not_return_itself()
        {
            // given
            Type type = typeof(IGenericParent<>);

            // when
            var immediateParents = type.GetImmediateParents().ToList();

            // then
            immediateParents.Should().BeEmpty();
        }

        [TestCaseSource("GetCollectionsWithDerivingTypes")]
        public void GetMostDerivedTypes_should_exclude_base_types(ICollection<Type> types)
        {
            // when
            var mostDerivedTypes = types.GetMostDerivedTypes().ToList();

            // then
            mostDerivedTypes.Should().HaveCount(3);
            mostDerivedTypes.Should().NotContain(typeof(IEntity));
            mostDerivedTypes.Should().NotContain(typeof(IAgent));
            mostDerivedTypes.Should().NotContain(typeof(IEntity));
        }

        private static IEnumerable GetCollectionsWithDerivingTypes()
        {
            yield return new TestCaseData(new List<Type>
                                              {
                                                  typeof(IEntity),
                                                  typeof(IDerived),
                                                  typeof(IAgent),
                                                  typeof(IPerson),
                                                  typeof(IAlsoPerson),
                                                  typeof(IDerivedLevel2)
                                              }).SetDescription("All parents first");
            yield return new TestCaseData(new List<Type>
                                              {
                                                  typeof(IPerson),
                                                  typeof(IAlsoPerson),
                                                  typeof(IDerivedLevel2),
                                                  typeof(IEntity),
                                                  typeof(IAgent),
                                                  typeof(IDerived)
                                              }).SetDescription("All parents last");
            yield return new TestCaseData(new List<Type>
                                              {
                                                  typeof(IPerson),
                                                  typeof(IDerived),
                                                  typeof(IAgent),
                                                  typeof(IAlsoPerson),
                                                  typeof(IDerivedLevel2),
                                                  typeof(IEntity)
                                              }).SetDescription("Parents mixed order");
        }
    }
}