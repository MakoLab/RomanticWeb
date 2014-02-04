using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RomanticWeb.DotNetRDF;
using RomanticWeb.Entities;
using RomanticWeb.JsonLd;
using RomanticWeb.Model;
using VDS.RDF;
using VDS.RDF.Parsing;

namespace RomanticWeb.Tests.JsonLd
{
    [TestFixture]
    public class JsonLdTestSuiteTests
    {
        private readonly string _testsRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"JsonLd\test-suite\tests");

        private JsonLdSerializer _serializer;

        [SetUp]
        public void Setup()
        {
            _serializer = new JsonLdSerializer();
        }

        [TestCaseSource("RdfToJsonTestCases")]
        public void RDF_to_JSON_test_suite(string input, string expected,bool userRdfType,bool useNativeTypes)
        {
            // given
            IEnumerable<EntityQuad> quads=GetQuads(input);

            // when
            string output = _serializer.FromRdf(quads,userRdfType,useNativeTypes);
            dynamic actualJson = JsonConvert.DeserializeObject(output);
            dynamic expectedJson = JsonConvert.DeserializeObject(File.ReadAllText(expected));

            LogExpectedAndActualJson(expectedJson.ToString(),actualJson.ToString());

            // then
            Assert.That(JToken.DeepEquals(actualJson, expectedJson));
        }

        private static void LogExpectedAndActualJson(string expectedJson,string actualJson)
        {
            Console.WriteLine("Result JSON:");
            Console.WriteLine(actualJson);

            Console.WriteLine("Expected JSON:");
            Console.WriteLine(expectedJson);
        }

        private IEnumerable<object> RdfToJsonTestCases()
        {
            string manifest=File.ReadAllText(Path.Combine(_testsRoot,"fromRdf-manifest.jsonld"));
            dynamic manifestJson=JsonConvert.DeserializeObject(manifest);

            foreach (var testManifest in manifestJson.sequence)
            {
                bool? useRdfType=null;
                bool? useNativeTypes=null;
                string input=Path.Combine(_testsRoot,(string)testManifest.input);
                string expect=Path.Combine(_testsRoot,(string)testManifest.expect);

                dynamic option=testManifest.option;
                if (option!=null)
                {
                    useNativeTypes=option.useNativeTypes;
                    useRdfType=option.useRdfType;
                }

                yield return new TestCaseData(input, expect,useRdfType.GetValueOrDefault(),useNativeTypes.GetValueOrDefault())
                        .SetName((string)testManifest["name"])
                        .SetDescription((string)testManifest["purpose"]);
            }
        }

        private IEnumerable<EntityQuad> GetQuads(string fileName)
        {
            var entityId=new EntityId("urn:jsonld:test");
            var store=new TripleStore();
            store.LoadFromFile(fileName,new NQuadsParser());

            return from triple in store.Triples
                   select
                       new EntityQuad(
                       entityId,
                       triple.Subject.WrapNode(entityId),
                       triple.Predicate.WrapNode(entityId),
                       triple.Object.WrapNode(entityId),
                       (triple.Graph == null)||(triple.Graph.BaseUri==null)?null:Node.ForUri(triple.Graph.BaseUri));
        }
    }
}