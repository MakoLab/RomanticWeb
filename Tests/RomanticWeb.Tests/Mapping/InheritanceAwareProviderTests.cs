using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Tests.Mapping
{
    [TestFixture]
    public class InheritanceAwareProviderTests
    {
        private readonly IOntologyProvider _ontologyProvider = new Mock<IOntologyProvider>().Object;
        private InheritanceTreeProvider _provider;

        [SetUp]
        public void Setup()
        {
            _provider=new TestingInheritanceTreeProvider();
        }

        [Test]
        public void Should_not_include_hidden_mapping_form_parent()
        {
            _provider.Properties.Should().HaveCount(3);
        }

        [Test]
        public void Should_map_to_uri_set_in_child_map()
        {
            // given
            var provider=_provider.Properties.Single(p => p.PropertyInfo.Name=="InParent3");

            // when
            var term=provider.GetTerm(_ontologyProvider);

            // then
            term.ToString().Should().Be("urn:override:parent3");
        }
    }
}