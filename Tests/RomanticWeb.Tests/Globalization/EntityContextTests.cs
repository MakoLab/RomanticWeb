using System;
using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RomanticWeb.Entities;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Model;
using RomanticWeb.Updates;

namespace RomanticWeb.Tests.Globalization
{
    [TestFixture]
    public class EntityContextTests
    {
        [Test]
        public void Should_provide_all_cultures_in_context()
        {
            // Given
            var context = CreateContext();

            // When
            var result = context.Cultures;

            // Then
            result.Should().HaveCount(2);
            result.Any(culture => culture.TwoLetterISOLanguageName == "pl").Should().BeTrue();
            result.Any(culture => culture.TwoLetterISOLanguageName == "en").Should().BeTrue();
        }

        private IEntityContext CreateContext()
        {
            var subject = new Uri("urn:subject");
            var predicate = new Uri("urn:predicate");
            var mapping = new Mock<IEntityMapping>();
            mapping.Setup(instance => instance.PropertyFor("Type")).Returns(new Mock<IPropertyMapping>().Object);
            var mappingsRepository = new Mock<IMappingsRepository>();
            mappingsRepository.Setup(instance => instance.MappingFor<ITypedEntity>()).Returns(mapping.Object);
            var store = new Mock<IEntityStore>();
            store.SetupGet(instance => instance.Quads).Returns(
                new[]
                {
                    new EntityQuad(new EntityId(subject), Node.ForUri(subject), Node.ForUri(predicate), Node.ForLiteral("test", "pl")),
                    new EntityQuad(new EntityId(subject), Node.ForUri(subject), Node.ForUri(predicate), Node.ForLiteral("test", "en")),
                    new EntityQuad(new EntityId(subject), Node.ForUri(subject), Node.ForUri(predicate), Node.ForLiteral("test"))
                });
            return new EntityContext(
                new Mock<IEntityContextFactory>().Object,
                mappingsRepository.Object,
                store.Object,
                new Mock<IEntitySource>().Object,
                new Mock<IBaseUriSelectionPolicy>().Object,
                new Mock<IRdfTypeCache>().Object,
                new Mock<IBlankNodeIdGenerator>().Object,
                new Mock<IResultTransformerCatalog>().Object,
                new Mock<IEntityCaster>().Object,
                new Mock<IDatasetChangesTracker>().Object,
                null);
        }
    }
}