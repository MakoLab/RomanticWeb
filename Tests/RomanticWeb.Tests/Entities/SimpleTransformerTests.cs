using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RomanticWeb.Converters;
using RomanticWeb.Entities;
using RomanticWeb.Entities.ResultAggregations;
using RomanticWeb.Entities.ResultPostprocessing;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Model;

namespace RomanticWeb.Tests.Entities
{
    [TestFixture]
    public class SimpleTransformerTests
    {
        private Mock<IResultAggregator> _aggregator;
        private Mock<INodeConverter> _converter;
        private Mock<IPropertyMapping> _mapping;
        private Mock<IEntityContext> _context;
        private Mock<IEntityProxy> _proxy;
        private IResultTransformer _transformer;

        [SetUp]
        protected void Setup()
        {
            _context = new Mock<IEntityContext>();
            _proxy = new Mock<IEntityProxy>();
            _converter = new Mock<INodeConverter>();
            _mapping = new Mock<IPropertyMapping>();
            _mapping.SetupGet(instance => instance.ReturnType).Returns(typeof(int));
            _mapping.SetupGet(instance => instance.Converter).Returns(_converter.Object);
            _aggregator = new Mock<IResultAggregator>(MockBehavior.Strict);
            _aggregator.Setup(instance => instance.Aggregate(It.IsAny<IEnumerable<object>>())).Returns<IEnumerable<object>>(values => values.First());
            _transformer = new SimpleTransformer(_aggregator.Object);
        }

        [Test]
        public void Should_transform_integer_literal_into_desired_type()
        {
            // Given
            _converter.Setup(instance => instance.Convert(It.IsAny<Node>(), _context.Object)).Returns<Node, IEntityContext>((node, context) => 1L);
            IEnumerable<Node> nodes = new Node[] { Node.ForLiteral("1", RomanticWeb.Vocabularies.Xsd.Integer) };

            // When
            object result = _transformer.FromNodes(_proxy.Object, _mapping.Object, _context.Object, nodes);

            // Then
            result.Should().BeOfType<Int32>();
        }
    }
}