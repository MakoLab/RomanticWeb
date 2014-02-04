using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
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

        [SetUp]
        public void Setup()
        {
            _serializer = new JsonLdSerializer();
        }

        [Test]
        public void Should_serialize_graph_with_blank_nodes()
        {
            // given
            var storeId = new EntityId("http://www.acme.com/#store");
            Stream resource = Resource.AsStream("TestCases.AcmeStore.ttl");
            object expected = JsonConvert.DeserializeObject(Resource.AsString("TestCases.AcmeStore.json"));

            // when
            var json = GetSerializedJson(storeId, resource);

            // then
            Assert.That(JToken.DeepEquals(json, (JToken)expected));
        }

        [Test]
        public void Should_serialize_simple_graph()
        {
            // given
            var storeId = new EntityId("http://www.acme.com/#store");
            Stream resource = Resource.AsStream("TestCases.SimpleObject.ttl");
            object expected = JsonConvert.DeserializeObject(Resource.AsString("TestCases.SimpleObject.json"));

            // when
            var json = GetSerializedJson(storeId, resource);

            // then
            Assert.That(JToken.DeepEquals(json, (JToken)expected));
        }

        [Test]
        public void Should_serialize_graph_with_nested_entity()
        {
            // given
            var storeId = new EntityId("http://www.acme.com/#store");
            Stream resource = Resource.AsStream("TestCases.NestedEntity.ttl");
            object expected = JsonConvert.DeserializeObject(Resource.AsString("TestCases.NestedEntity.json"));

            // when
            var json = GetSerializedJson(storeId, resource);

            // then
            Assert.That(JToken.DeepEquals(json, (JToken)expected));
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
    }
}