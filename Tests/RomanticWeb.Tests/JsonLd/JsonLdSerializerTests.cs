using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;
using Resourcer;
using RomanticWeb.DotNetRDF;
using RomanticWeb.Entities;
using RomanticWeb.JsonLd;
using RomanticWeb.Model;
using VDS.RDF;
using VDS.RDF.Parsing;

namespace RomanticWeb.Tests.JsonLd
{
    [TestFixture]
    public class JsonLdSerializerTests
    {
        private JsonLdSerializer _serializer;

        private string _testsRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"JsonLd\\test_cases");

        [SetUp]
        public void Setup()
        {
            _serializer = new JsonLdSerializer();
        }

        [TestCaseSource("RdfToJsonTestCases")]
        public void RDF_to_JSON_test_suite()
        {
            // given

            // when

            // then
            Assert.Inconclusive();
        }

        [Test]
        public void Should_serialize_blank_node_as_top_level_object()
        {
            // given
            var storeId = new EntityId("http://www.acme.com/#store");
            Stream resource = Resource.AsStream("TestCases.AcmeStore.ttl");

            // when
            var json = GetSerializedJson(storeId, resource);

            // then
            Assert.That(json.Count, Is.EqualTo(2));
        }

        [Test]
        public void Should_serialize_simple_graph_with_properties()
        {
            // given
            var storeId = new EntityId("http://www.acme.com/#store");
            Stream resource = Resource.AsStream("TestCases.SimpleObject.ttl");

            // when
            var json = GetSerializedJson(storeId, resource);

            // then
            Assert.That(json.name.Value, Is.EqualTo("Hepp's Happy Burger Restaurant"));
            Assert.That(json.category.Value, Is.EqualTo("Food/Fast"));
            Assert.That(json.description.Value, Is.EqualTo("They serve massive, unhealthy burgers"));
        }

        [Test]
        public void Should_serialize_simple_graph_with_context()
        {
            // given
            var storeId = new EntityId("http://www.acme.com/#store");
            Stream resource = Resource.AsStream("TestCases.SimpleObject.ttl");

            // when
            dynamic json = GetSerializedJson(storeId, resource);

            // then
            Assert.That(json["@context"], Is.Not.Null);
            var context = json["@context"];
            Assert.That(context.gr, Is.EqualTo("http://purl.org/goodrelations/v1#"));
            Assert.That(context.name, Is.EqualTo("http://purl.org/goodrelations/v1#name"));
            Assert.That(context.category, Is.EqualTo("http://purl.org/goodrelations/v1#category"));
            Assert.That(context.description, Is.EqualTo("http://purl.org/goodrelations/v1#description"));
        }

        [Test]
        public void Should_serialize_ids_for_URI_resources()
        {
            // given
            var storeId = new EntityId("http://www.acme.com/#store");
            Stream resource = Resource.AsStream("TestCases.SimpleObject.ttl");

            // when
            dynamic json = GetSerializedJson(storeId, resource);

            // then
            Assert.That(json["@id"], Is.EqualTo("http://www.acme.com/#store"));
        }

        [Test]
        public void Should_serialize_nested_entity_as_top_level_object()
        {
            // given
            var storeId = new EntityId("http://www.acme.com/#store");
            Stream resource = Resource.AsStream("TestCases.SimpleObject.ttl");

            // when
            dynamic json = GetSerializedJson(storeId, resource);

            // then
            Assert.That(json[JsonLdSerializer.Graph].Count, Is.EqualTo(2));
        }

        private dynamic GetSerializedJson(EntityId id, Stream resource)
        {
            string json=_serializer.FromRdf(GetQuads(id, resource));

            object deserializeObject = JsonConvert.DeserializeObject(json);
            Console.WriteLine("Result JSON:");
            Console.WriteLine(deserializeObject.ToString());

            return deserializeObject;
        }

        private IEnumerable<EntityQuad> GetQuads(EntityId entityId, Stream resource)
        {
            IGraph graph = new Graph
            {
                BaseUri = entityId.Uri
            };
            using (var streamReader=new StreamReader(resource))
            {
                new TurtleParser().Load(graph, streamReader);
            }

            return from triple in graph.Triples
                   select
                       new EntityQuad(
                       entityId,
                       triple.Subject.WrapNode(entityId),
                       triple.Predicate.WrapNode(entityId),
                       triple.Object.WrapNode(entityId),
                       triple.Graph == null ? null : Node.ForUri(triple.Graph.BaseUri));
        } 

        private IEnumerable<object> RdfToJsonTestCases()
        {
            yield break;
        }
    }
}