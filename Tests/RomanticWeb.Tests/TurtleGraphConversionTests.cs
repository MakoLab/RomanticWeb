using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RomanticWeb.DotNetRDF;
using VDS.RDF;

namespace RomanticWeb.Tests
{
    public class TurtleGraphConversionTests
    {
        private ITripleStore store;

        [SetUp]
        public void Setup()
        {
            store = new TripleStore();
            store.LoadFromEmbeddedResource(System.String.Format("{0}.ttl, {1}", GetType().FullName, GetType().Assembly.FullName), new Uri("http://app.magi/graphs"));
        }

        [Test]
        public void Should_generate_graphs_automatically()
        {
            Assert.That(store.Graphs.Any(item => item.BaseUri.AbsoluteUri == "http://test/individual1"));
        }
    }
}