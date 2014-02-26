using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
        private readonly string _testsRoot=Path.Combine(AppDomain.CurrentDomain.BaseDirectory,@"JsonLd\test-suite\tests");

        private IJsonLdProcessor _processor;

        [SetUp]
        public void Setup()
        {
            _processor=new JsonLdProcessor();
        }

        [TestCaseSource("RdfToJsonTestCases")]
        public void RDF_to_JSON_test_suite(string input,string expectedPath,JObject options)
        {
            // given
            IEnumerable<EntityQuad> quads=GetQuads(input);
            Func<string> processDatasetFunc=() => _processor.FromRdf(quads);
            if (options!=null)
            {
                if (options.Property("useRdfType")!=null)
                {
                    processDatasetFunc=() => _processor.FromRdf(quads,userRdfType:(bool)options["useRdfType"]);
                }
                if (options.Property("useNativeTypes")!=null)
                {
                    processDatasetFunc=() => _processor.FromRdf(quads,useNativeTypes:(bool)options["useNativeTypes"]);
                }
            }

            // when, then
            ExecuteTest(processDatasetFunc,expectedPath);
        }

        [Ignore]
        [TestCaseSource("ExpandTestsCases")]
        public void Expand_test_suite(string input,string expectedPath,JObject options)
        {
            // given
            string inputJson=File.ReadAllText(input);
            var jsonOptions=new JsonLdOptions() { BaseUri=new Uri("http://json-ld.org/test-suite/tests/"+System.IO.Path.GetFileName(input)) };
            Func<string> expandTestFunc=() => _processor.Expand(inputJson,jsonOptions);

            if (options!=null)
            {
                if (options.Property("base")!=null)
                {
                    jsonOptions.BaseUri=new Uri((string)options["base"]);
                }

                if (options.Property("expandContext")!=null)
                {
                    jsonOptions.ExpandContext=File.ReadAllText(Path.Combine(_testsRoot,(string)options["expandContext"]));
                }
            }

            // when, then
            ExecuteTest(expandTestFunc,expectedPath);
        }

        [TestCaseSource("JsonToRdfTestsCases")]
        public void JSON_to_RDF_test_suite(string input,string expectedPath,JObject options)
        {
            // given
            string inputJson=File.ReadAllText(input);
            var jsonOptions=new JsonLdOptions() { BaseUri=new Uri("http://json-ld.org/test-suite/tests/"+System.IO.Path.GetFileName(input)) };
            Func<IEnumerable<EntityQuad>> processTestFunc=() => _processor.ToRdf(inputJson,jsonOptions);
            if (options!=null)
            {
                if (options.Property("produceGeneralizedRdf")!=null)
                {
                    processTestFunc=() => _processor.ToRdf(inputJson,jsonOptions,produceGeneralizedRdf:(bool)options["produceGeneralizedRdf"]);
                }
            }

            // given
            IEnumerable<EntityQuad> expectedStore=GetQuads(expectedPath,(options!=null)&&(options.Property("produceGeneralizedRdf")!=null)?!(bool)options["produceGeneralizedRdf"]:true);

            // when
            IEnumerable<EntityQuad> actualStore=processTestFunc();

            // then
            Assert.That(actualStore,Is.EquivalentTo(expectedStore));
        }

        private static void ExecuteTest(Func<string> getTestResult,string expectedPath)
        {
            // given
            dynamic expectedJson=JsonConvert.DeserializeObject(File.ReadAllText(expectedPath));

            // when
            dynamic actualJson=JsonConvert.DeserializeObject(getTestResult());

            // then
            LogExpectedAndActualJson(expectedJson.ToString(),actualJson.ToString());
            Assert.That(JToken.DeepEquals(actualJson,expectedJson));
        }

        private static void LogExpectedAndActualJson(string expectedJson,string actualJson)
        {
            Console.WriteLine("Result JSON:");
            Console.WriteLine(actualJson);

            Console.WriteLine("Expected JSON:");
            Console.WriteLine(expectedJson);
        }

        private static IEnumerable<TestCaseData> ReadTestManifests(string namePrefix,string manifestsPath,string rootManifestName)
        {
            var manifest=File.ReadAllText(Path.Combine(manifestsPath,rootManifestName));
            JObject manifestJson=JObject.Parse(manifest);
            foreach (var testManifest in manifestJson["sequence"])
            {
                string input=Path.Combine(manifestsPath,(string)testManifest["input"]);
                string expect=Path.Combine(manifestsPath,(string)testManifest["expect"]);

                yield return new TestCaseData(input,expect,testManifest["option"])
                        .SetName(System.String.Format("{2} {0}: {1}",testManifest["@id"].ToString().Substring(3),(string)testManifest["name"],namePrefix))
                        .SetDescription((string)testManifest["purpose"]);
            }
        }

        private IEnumerable<TestCaseData> ExpandTestsCases()
        {
            return ReadTestManifests("Expand test",_testsRoot,"expand-manifest.jsonld");
        }

        private IEnumerable<TestCaseData> RdfToJsonTestCases()
        {
            return ReadTestManifests("RDF to JSON-LD test",_testsRoot,"fromRdf-manifest.jsonld");
        }

        private IEnumerable<TestCaseData> JsonToRdfTestsCases()
        {
            return ReadTestManifests("JSON-LD to RDF test",_testsRoot,"toRdf-manifest.jsonld");
        }

        private IEnumerable<EntityQuad> GetQuads(string fileName,bool useNQuads=true)
        {
            var store=new TripleStore();
            if (useNQuads)
            {
                new NQuadsParser().Load(store,fileName);
            }
            else
            {
                IGraph graph=new Graph();
                new Notation3Parser().Load(graph,fileName);
                store.Add(graph);
            }

            IDictionary<string,string> bnodes=CreateGraphMap(store,fileName,useNQuads);

            return from triple in store.Triples
                   let entityId=(triple.Subject is IUriNode?new EntityId(((IUriNode)triple.Subject).Uri):new BlankId(((IBlankNode)triple.Subject).InternalID))
                   select
                       new EntityQuad(
                       entityId,
                       WrapNode(triple.Subject),
                       WrapNode(triple.Predicate),
                       WrapNode(triple.Object),
                       (triple.Graph==null)||(triple.Graph.BaseUri==null)?null:
                        (Regex.IsMatch(triple.Graph.BaseUri.AbsoluteUri,"[a-zA-Z0-9_]+://")?Node.ForUri(triple.Graph.BaseUri):
                            Node.ForBlank(bnodes[triple.Graph.BaseUri.AbsoluteUri],null,null)));
        }

        /// <summary>Creates a map graph names between store and the expected file.</summary>
        /// <remarks>This is due to fact that dotNetRDF expects an <see cref="Uri" /> as named graph, while the graph name can be also a blank node identifier.</remarks>
        private IDictionary<string,string> CreateGraphMap(ITripleStore store,string fileName,bool useNQuads=true)
        {
            IDictionary<string,string> result=new Dictionary<string,string>();
            MemoryStream buffer=new MemoryStream(1024);
            if (useNQuads)
            {
                new VDS.RDF.Writing.NQuadsWriter().Save(store,new StreamWriter(buffer));
            }
            else
            {
                new VDS.RDF.Writing.Notation3Writer().Save(store.Graphs.First(),new StreamWriter(buffer));
            }

            MatchCollection matches=Regex.Matches(System.Text.UTF8Encoding.UTF8.GetString(buffer.ToArray()),"\\<(?<bnode>nquads\\:bnode\\:[0-9]+)\\>");
            FileStream fileStream=null;
            try
            {
                fileStream=File.Open(fileName,FileMode.Open,FileAccess.Read);
                int index=0;
                foreach (Match match in Regex.Matches(new StreamReader(fileStream).ReadToEnd(),"\\<[^>]+\\> \\<[^>]+\\> ((\\<[^>]+\\>)|(\"[^\"]+\")) _:(?<bnode>[a-zA-Z0-9]+) \\."))
                {
                    if (!result.ContainsKey(matches[index].Groups["bnode"].Value))
                    {
                        result[matches[index].Groups["bnode"].Value]=match.Groups["bnode"].Value;
                    }
                    index++;
                }
            }
            catch
            {
            }
            finally
            {
                if (fileStream!=null)
                {
                    fileStream.Close();
                }
            }

            return result;
        }

        private Node WrapNode(INode node)
        {
            var literal=node as ILiteralNode;
            if (literal!=null)
            {
                if (literal.DataType!=null)
                {
                    return Node.ForLiteral(literal.Value,literal.DataType);
                }

                if (literal.Language!=null)
                {
                    return Node.ForLiteral(literal.Value,literal.Language);
                }

                return Node.ForLiteral(literal.Value);
            }

            var uriNode=node as IUriNode;
            if (uriNode!=null)
            {
                return Node.ForUri(uriNode.Uri);
            }

            var blankNode=node as IBlankNode;
            if (blankNode!=null)
            {
                return Node.ForBlank(blankNode.InternalID,null,null);
            }

            throw new ArgumentException("The node was neither URI, literal nor blank","node");
        }
    }
}