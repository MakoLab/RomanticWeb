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

        private IJsonLdProcessor _processor;

        [SetUp]
        public void Setup()
        {
            _processor = new JsonLdProcessor();
        }

        [TestCaseSource("RdfToJsonTestCases")]
        public void RDF_to_JSON_test_suite(string input,string expectedPath,dynamic options)
        {
            // given
            IEnumerable<EntityQuad> quads=GetQuads(input);
            Func<string> processDatasetFunc=() => _processor.FromRdf(quads);
            if (options!=null)
            {
                if (((bool?)options.useRdfType).HasValue)
                {
                    processDatasetFunc=() => _processor.FromRdf(quads,userRdfType: true);
                }
                else if (((bool?)options.useNativeTypes).HasValue)
                {
                    processDatasetFunc=() => _processor.FromRdf(quads,useNativeTypes: true);
                }
            }

            // when, then
            ExecuteTest(processDatasetFunc,expectedPath);
        }

        [TestCaseSource("ExpandTestsCases")]
        public void Expand_test_suite(string input,string expectedPath,dynamic options)
        {
            // given
            string inputJson=File.ReadAllText(input);
            var jsonOptions=new JsonLdOptions();
            Func<string> expandTestFunc=() => _processor.Expand(inputJson,jsonOptions);

            if (options!=null)
            {
                if (options.@base!=null)
                {
                    jsonOptions.BaseUri=new Uri((string)options.@base);
                }

                if (options.expandContext!=null)
                {
                    jsonOptions.ExpandContext=File.ReadAllText(Path.Combine(_testsRoot,(string)options.expandContext));
                }
            }

            // when, then
            //ExecuteTest(expandTestFunc,expectedPath);
        }

        private static void ExecuteTest(Func<string> getTestResult,string expectedPath)
        {
            // given
            dynamic expectedJson = JsonConvert.DeserializeObject(File.ReadAllText(expectedPath));

            // when
            dynamic actualJson = JsonConvert.DeserializeObject(getTestResult());

            // then
            LogExpectedAndActualJson(expectedJson.ToString(), actualJson.ToString());
            Assert.That(JToken.DeepEquals(actualJson, expectedJson));
        }

        private static void LogExpectedAndActualJson(string expectedJson,string actualJson)
        {
            Console.WriteLine("Result JSON:");
            Console.WriteLine(actualJson);

            Console.WriteLine("Expected JSON:");
            Console.WriteLine(expectedJson);
        }

        private static IEnumerable<TestCaseData> ReadTestManifests(string manifestsPath, string rootManifestName)
        {
            var manifest=File.ReadAllText(Path.Combine(manifestsPath,rootManifestName));
            dynamic manifestJson=JObject.Parse(manifest);

            int testIndex = 1;
            foreach (var testManifest in manifestJson.sequence)
            {
                string input=Path.Combine(manifestsPath,(string)testManifest.input);
                string expect=Path.Combine(manifestsPath,(string)testManifest.expect);

                yield return new TestCaseData(input, expect, testManifest.option)
                        .SetName(string.Format("{0:00}: {1}", testIndex++, (string)testManifest["name"]))
                        .SetDescription((string)testManifest["purpose"]);
            }
        } 

        private IEnumerable<TestCaseData> ExpandTestsCases()
        {
            return ReadTestManifests(_testsRoot,"expand-manifest.jsonld");
        }

        private IEnumerable<TestCaseData> RdfToJsonTestCases()
        {
            return ReadTestManifests(_testsRoot, "fromRdf-manifest.jsonld");
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