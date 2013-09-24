using System;
using VDS.RDF;

namespace RomanticWeb.Tests.Helpers
{
    public static class TripleStoreExtensions
    {
        public static void LoadTestFile(this ITripleStore store, string fileName)
        {
            string resource = GetResourceName(fileName);
            store.LoadFromEmbeddedResource(resource);
        }

        public static void LoadTestFile(this IGraph graph, string fileName)
        {
            graph.LoadFromEmbeddedResource(GetResourceName(fileName));
        }

        public static void LoadTestFile(this ITripleStore store, string fileName, Uri graphUri)
        {
            var graph=new Graph();
            graph.BaseUri=graphUri;
            graph.LoadTestFile(fileName);
            store.Add(graph);
        }

        private static string GetResourceName(string fileName)
        {
            return string.Format("RomanticWeb.Tests.TestGraphs.{0}, RomanticWeb.Tests", fileName);
        }
    }
}