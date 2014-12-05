using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RomanticWeb.Collections;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.TestEntities;
using RomanticWeb.TestEntities.Inheritance;

namespace RomanticWeb.Collections.Tests
{
    [TestFixture]
    public class TopologicSortTests
    {
        [Test]
        public void Should_sort_types_in_correct_order()
        {
            // Given
            IList<KeyValuePair<Type, IList<IEntityMappingProvider>>> types = new List<KeyValuePair<Type, IList<IEntityMappingProvider>>>(
                new KeyValuePair<Type, IList<IEntityMappingProvider>>[]
                {
                    new KeyValuePair<Type, IList<IEntityMappingProvider>>(typeof(IEvenMoreSpecificContainer), new List<IEntityMappingProvider>()),
                    new KeyValuePair<Type, IList<IEntityMappingProvider>>(typeof(ISpecificContainer), new List<IEntityMappingProvider>()),
                    new KeyValuePair<Type, IList<IEntityMappingProvider>>(typeof(IGenericContainer<>), new List<IEntityMappingProvider>()),
                    new KeyValuePair<Type, IList<IEntityMappingProvider>>(typeof(IGenericContainer), new List<IEntityMappingProvider>()),
                    new KeyValuePair<Type, IList<IEntityMappingProvider>>(typeof(IEntityWithDictionary), new List<IEntityMappingProvider>())
                });

            // When
            IList<KeyValuePair<Type, IList<IEntityMappingProvider>>> sorted = types.TopologicSort().Reverse().ToList();

            // Then
            if (sorted[0].Key == typeof(IEntityWithDictionary))
            {
                sorted[1].Key.Should().Be(typeof(IGenericContainer));
                sorted[2].Key.Should().Be(typeof(IGenericContainer<>));
                sorted[3].Key.Should().Be(typeof(ISpecificContainer));
                sorted[4].Key.Should().Be(typeof(IEvenMoreSpecificContainer));
            }
            else
            {
                sorted[0].Key.Should().Be(typeof(IGenericContainer));
                sorted[1].Key.Should().Be(typeof(IGenericContainer<>));
                sorted[2].Key.Should().Be(typeof(ISpecificContainer));
                sorted[3].Key.Should().Be(typeof(IEvenMoreSpecificContainer));
            }
        }
    }
}