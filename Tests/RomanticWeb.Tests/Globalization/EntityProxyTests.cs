using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RomanticWeb.Entities;
using RomanticWeb.Entities.ResultPostprocessing;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Model;
using RomanticWeb.NamedGraphs;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Tests.Globalization
{
    [TestFixture]
    public class EntityProxyTests
    {
        [Test]
        [TestCase("pl")]
        [TestCase("en")]
        public void Should_return_localized_value_in_correct_language(string language)
        {
            // Given
            var values = new[] { Node.ForLiteral("test-pl", "pl"), Node.ForLiteral("test-en", "en"), Node.ForLiteral("test", Xsd.String) };
            var proxy = CreateProxy(language, values);

            // When
            var result = (string)((dynamic)proxy).Test;

            // Then
            result.Should().Be(values.First(value => value.Language == language).Literal);
        }

        private EntityProxy CreateProxy(string language, IEnumerable<Node> values)
        {
            var entityId = new EntityId("urn:test");
            var predicate = new Uri("urn:predicate");
            var store = new Mock<IEntityStore>();
            store.Setup(instance => instance.GetObjectsForPredicate(entityId, predicate, It.IsAny<Uri>())).Returns(values);
            var context = new Mock<IEntityContext>();
            context.SetupGet(instance => instance.CurrentCulture).Returns(CultureInfo.GetCultureInfo(language));
            context.SetupGet(instance => instance.Store).Returns(store.Object);
            var entity = new Entity(entityId, context.Object);
            context.Setup(instance => instance.InitializeEnitity(entity));
            var propertyMapping = new Mock<IPropertyMapping>();
            propertyMapping.SetupGet(instance => instance.Uri).Returns(predicate);
            var mapping = new Mock<IEntityMapping>();
            mapping.Setup(instance => instance.PropertyFor("Test")).Returns(propertyMapping.Object);
            var transformer = new Mock<IResultTransformer>();
            var catalog = new Mock<IResultTransformerCatalog>();
            catalog.Setup(instance => instance.GetTransformer(propertyMapping.Object)).Returns(transformer.Object);
            var selector = new Mock<INamedGraphSelector>();
            selector.Setup(instance => instance.SelectGraph(entityId, mapping.Object, propertyMapping.Object)).Returns(new Uri("urn:graph"));
            var proxy = new EntityProxy(entity, mapping.Object, catalog.Object, selector.Object);
            transformer.Setup(instance => instance.FromNodes(proxy, propertyMapping.Object, context.Object, It.IsAny<IEnumerable<Node>>()))
                .Returns<IEntityProxy, IPropertyMapping, IEntityContext, IEnumerable<Node>>(
                    (prx, pm, ctx, nodes) => nodes.Select(node => (node.IsLiteral ? (object)node.Literal : (object)node.Uri)).FirstOrDefault());

            return proxy;
        }
    }
}