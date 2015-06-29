using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Core;
using NUnit.Framework;
using RomanticWeb.Entities;
using RomanticWeb.NamedGraphs;

namespace RomanticWeb.Tests.NamedGraphs
{
    [TestFixture]
    public class BaseUriNamedGraphSelectorTests
    {
        private const string BaseUri = "http://temp.uri/segment";
        private INamedGraphSelector _graphSelector;

        [Test]
        [TestCase(BaseUri, false)]
        [TestCase(BaseUri + "/test", false)]
        [TestCase(BaseUri, true)]
        [TestCase(BaseUri + "#entity", false)]
        public void Should_provide_graph_name_correctly(string resourceUri, bool isBlankId)
        {
            var id = new EntityId(new Uri(BaseUri));
            if (isBlankId)
            {
                id = new BlankId("bnode001", id);
            }

            var result = _graphSelector.SelectGraph(id, null, null);

            result.Should().Be(new Uri(BaseUri));
        }

        [SetUp]
        public void Setup()
        {
            _graphSelector = new BaseUriNamedGraphSelector(new Uri(BaseUri));
        }

        [TearDown]
        public void Teardown()
        {
            _graphSelector = null;
        }
    }
}