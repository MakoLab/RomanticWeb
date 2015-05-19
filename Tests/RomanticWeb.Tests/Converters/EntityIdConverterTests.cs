using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RomanticWeb.Converters;
using RomanticWeb.Entities;
using RomanticWeb.Model;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Tests.Converters
{
    public class EntityIdConverterTests
    {
        private EntityIdConverter _converter;
        private Mock<IBaseUriSelectionPolicy> _baseUriSelectionPolicy;

        [SetUp]
        public void Setup()
        {
            _baseUriSelectionPolicy = new Mock<IBaseUriSelectionPolicy>();
            _converter = new EntityIdConverter(_baseUriSelectionPolicy.Object);
        }

        [Test]
        [TestCase("/test", "http://test.org/")]
        public void Should_convert_to_absolute_uri(string relativeUri, string absoluteUri)
        {
            // Given
            _baseUriSelectionPolicy.Setup(instance => instance.SelectBaseUri(It.IsAny<EntityId>())).Returns(new Uri(absoluteUri, UriKind.Absolute));

            // When
            Node node = _converter.ConvertBack(new EntityId(relativeUri), new Mock<IEntityContext>().Object);

            // Then
            node.Should().NotBeNull();
            node.Uri.Should().Be(new Uri(new Uri(absoluteUri, UriKind.Absolute), new Uri(relativeUri, UriKind.Relative)));
        }
    }
}