using System;
using System.Configuration;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RomanticWeb.Configuration;

namespace RomanticWeb.Tests
{
    [TestFixture]
    public class ConfigurationTests
    {
        private ConfigurationSectionHandler _configuration;

        [SetUp]
        public void Setup()
        {
            _configuration=(ConfigurationSectionHandler)ConfigurationManager.GetSection("romanticWeb");
        }

        [Test]
        public void Should_contain_added_assemlies()
        {
            _configuration.MappingAssemblies.Count.Should().Be(2);
            _configuration.MappingAssemblies.Cast<MappingAssemblyElement>()
                          .Select(e => e.Assembly)
                          .Should().ContainInOrder("Magi.Balthazar.Contracts","Magi.Web");
        }

        [Test]
        public void Should_contain_added_ontology_prefixes()
        {
            _configuration.Ontologies.Count.Should().Be(2);
            _configuration.Ontologies.Cast<OntologyElement>()
                          .Select(e => e.Prefix)
                          .Should().ContainInOrder("lemon","frad");
            _configuration.Ontologies.Cast<OntologyElement>()
                          .Select(e => e.Uri.ToString())
                          .Should().ContainInOrder("http://www.monnet-project.eu/lemon#","http://iflastandards.info/ns/fr/frad/");
        }

        [Test]
        public void Should_contain_default_base_uri()
        {
            _configuration.BaseUris.Default.Should().Be(new Uri("http://www.romanticweb.com/"));
        }

        [Test]
        public void Should_contain_meta_graph_uri()
        {
            _configuration.MetaGraphUri.Should().Be(new Uri("http://meta.romanticweb.com/"));
        }

        [Test,ExpectedException(typeof(ConfigurationErrorsException))]
        public void Should_require_meta_graph_uri()
        {
            ConfigurationManager.GetSection("missingMetaGraph");
        }

        [Test]
        public void Empty_configuration_should_be_populated()
        {
            // given
            var emptyConfiguration=(ConfigurationSectionHandler)ConfigurationManager.GetSection("romanticWebDefaults");

            // then
            emptyConfiguration.Ontologies.Should().BeEmpty();
            emptyConfiguration.MappingAssemblies.Should().BeEmpty();
            emptyConfiguration.MetaGraphUri.Should().Be(new Uri("http://graphs.example.com"));
            emptyConfiguration.BaseUris.Default.Should().BeNull();
        }
    }
}