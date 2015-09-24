using System;
using System.IO;
using System.Net;
using System.Text;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RomanticWeb.Entities;
using RomanticWeb.LinkedData;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Ontologies;
using RomanticWeb.TestEntities.Foaf;
using VDS.RDF;
using VDS.RDF.Writing;

namespace RomanticWeb.Tests.DotNetRDF
{
    [TestFixture]
    public class UrlMatchingResourceResolutionStrategyTests
    {
        private static readonly Uri BaseUri = new Uri("http://temp.uri/");
        private UrlMatchingResourceResolutionStrategy _strategy;

        [Test]
        public void Should_load_external_resource()
        {
            var id = new EntityId(new Uri(BaseUri, "test"));
            var graph = new Graph();
            graph.Assert(graph.CreateUriNode(id.Uri), graph.CreateUriNode(new Uri(BaseUri, "predicate")), graph.CreateLiteralNode("test"));
            WebRequest request = null;
            Stream data = new MemoryStream();
            SerializeTriples(graph, data);
            _strategy = new UrlMatchingResourceResolutionStrategy(CreateOntologyProvider(), new[] { typeof(IPerson).Assembly }, new[] { BaseUri }, uri => request = CreateWebRequest(uri, data));

            var result = _strategy.Resolve(id);

            result.Should().NotBeNull();
            result.Context.Store.GetEntityQuads(id).Should().HaveCount(1);
            Mock.Get(request).Verify(instance => instance.GetResponse(), Times.Once);
        }

        [Test]
        public void Should_load_all_external_resources()
        {
            var id = new EntityId(new Uri(BaseUri, "#test"));
            var another = new EntityId(new Uri(BaseUri, "#another"));
            var graph = new Graph();
            graph.Assert(graph.CreateUriNode(id.Uri), graph.CreateUriNode(new Uri(BaseUri, "predicate")), graph.CreateLiteralNode("test"));
            graph.Assert(graph.CreateUriNode(another.Uri), graph.CreateUriNode(new Uri(BaseUri, "predicate")), graph.CreateLiteralNode("test"));
            WebRequest request = null;
            Stream data = new MemoryStream();
            SerializeTriples(graph, data);
            _strategy = new UrlMatchingResourceResolutionStrategy(CreateOntologyProvider(), new[] { typeof(IPerson).Assembly }, new[] { BaseUri }, uri => request = CreateWebRequest(uri, data));

            var result = _strategy.Resolve(id);
            _strategy.Resolve(another);

            result.Should().NotBeNull();
            result.Context.Store.GetEntityQuads(id).Should().HaveCount(2);
            Mock.Get(request).Verify(instance => instance.GetResponse(), Times.Once);
        }

        [Test]
        public void Should_not_load_external_resource()
        {
            _strategy = new UrlMatchingResourceResolutionStrategy(CreateOntologyProvider(), null, new[] { BaseUri });

            var result = _strategy.Resolve(new EntityId(new Uri("http://test.uri/")));

            result.Should().BeNull();
        }

        private static WebRequest CreateWebRequest(Uri uri, Stream data)
        {
            var response = new Mock<WebResponse>(MockBehavior.Strict);
            response.Setup(instance => instance.GetResponseStream()).Returns(data);
            response.SetupGet(instance => instance.ContentType).Returns("text/turtle");
            var result = new Mock<WebRequest>(MockBehavior.Strict);
            result.Setup(instance => instance.GetResponse()).Returns(response.Object);
            return result.Object;
        }

        private static void SerializeTriples(IGraph graph, Stream target)
        {
            using (var writer = new StreamWriter(target, Encoding.UTF8, 4096, true))
            {
                var serializer = new CompressingTurtleWriter();
                serializer.Save(graph, writer);
            }

            target.Seek(0, SeekOrigin.Begin);
        }

        private IOntologyProvider CreateOntologyProvider()
        {
            var result = new Mock<IOntologyProvider>(MockBehavior.Strict);
            result.Setup(instance => instance.ResolveUri(It.IsAny<string>(), It.IsAny<string>())).Returns<string, string>((prefix, term) => new Uri("http://temp.uri/" + term));
            return result.Object;
        }
    }
}