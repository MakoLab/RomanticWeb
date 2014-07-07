using NUnit.Framework;
using RomanticWeb.NamedGraphs;

namespace RomanticWeb.Tests.NamedGraphs
{
    [TestFixture]
    public class NamedGraphSelectorTests
    {
        private NamedGraphSelector _namedGraphSelector;

        [SetUp]
        public void Setup()
        {
            _namedGraphSelector = new NamedGraphSelector();
        }

        [TearDown]
        public void Teardown()
        {
        }

        [Test]
        public void Should_use_default_selection_policy()
        {
            // given

            // when

            // then
            Assert.Inconclusive();
        }
    }
}