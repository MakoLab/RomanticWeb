using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RomanticWeb.ComponentModel;
using RomanticWeb.DotNetRDF;
using RomanticWeb.LinkedData;
using RomanticWeb.Mapping;
using RomanticWeb.NamedGraphs;
using RomanticWeb.TestEntities.LinkedData;
using RomanticWeb.Tests.Stubs;
using RomanticWeb.Vocabularies;
using VDS.RDF;

namespace RomanticWeb.Tests.IntegrationTests.LinkedData
{
    [TestFixture]
    public class LinkedDataTests
    {
        private static readonly Uri MetaGraphUri = new Uri("meta://graph");
        private static readonly Uri EntityGraphUri = new Uri("http://test.uri/resource");
        private static readonly Uri[] BaseUris = new[] { new Uri("http://temp.uri/") };
        private IEntityContext _entityContext;

        [Test]
        public void Should_provide_entity_from_underlying_store()
        {
            var entity = _entityContext.Load<IKnowSomething>(EntityGraphUri);

            entity.Should().NotBeNull();
        }

        [Test]
        public void Should_provide_entity_from_external_source()
        {
            var entity = _entityContext.Load<IKnowSomething>(EntityGraphUri).IWontTell;

            entity.Should().NotBeNull();
            entity.Label.Should().Be("test");
        }

        [SetUp]
        public void Setup()
        {
            _entityContext = CreateEntityContext();
        }

        [TearDown]
        public void Teardown()
        {
            _entityContext = null;
        }

        private static ITripleStore CreateTripleStore()
        {
            var tripleStore = new TripleStore();
            var entityGraph = new Graph() { BaseUri = EntityGraphUri };
            entityGraph.Assert(
                entityGraph.CreateUriNode(entityGraph.BaseUri),
                entityGraph.CreateUriNode(typeof(IKnowSomething).GetProperty("IWontTell").GetCustomAttribute<RomanticWeb.Mapping.Attributes.PropertyAttribute>().Uri),
                entityGraph.CreateUriNode(new Uri(BaseUris.First(), "about")));
            var metaGraph = new Graph() { BaseUri = MetaGraphUri };
            metaGraph.Assert(metaGraph.CreateUriNode(entityGraph.BaseUri), metaGraph.CreateUriNode(Foaf.primaryTopic), metaGraph.CreateUriNode(entityGraph.BaseUri));
            tripleStore.Add(metaGraph);
            tripleStore.Add(entityGraph);
            return tripleStore;
        }

        private static IEntityContext CreateEntityContext()
        {
            var entityContextFactory = new EntityContextFactory()
                .WithMetaGraphUri(MetaGraphUri)
                .WithMappings(builder => builder.FromAssemblyOf<IKnowSomething>())
                .WithDefaultOntologies()
                .WithOntology(new IntegrationTestsBase.ChemOntology())
                .WithOntology(new IntegrationTestsBase.LifeOntology())
                .WithOntology(new TestOntologyProvider(false))
                .WithNamedGraphSelector(new NamedGraphSelector())
                .WithDependenciesInternal<BaseUriResolutionStrategyComposition>()
                .WithDotNetRDF(CreateTripleStore());
            var resolutionStrategy = new UrlMatchingResourceResolutionStrategy(
                entityContextFactory.Ontologies,
                entityContextFactory.MappingModelVisitors.OfType<BaseUriMappingModelVisitor>().First().MappingAssemblies,
                BaseUris,
                CreateWebRequest);
            return entityContextFactory
                .WithResourceResolutionStrategy(resolutionStrategy)
                .CreateContext();
        }

        private static WebRequest CreateWebRequest(Uri baseUri)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(String.Format("<{0}about> <{1}> \"test\"^^<{2}> .", BaseUris.First(), Rdfs.label, Xsd.String)));
            var response = new Mock<WebResponse>(MockBehavior.Strict);
            response.Setup(instance => instance.GetResponseStream()).Returns(stream);
            response.SetupGet(instance => instance.ContentType).Returns("text/turtle");
            var result = new Mock<WebRequest>(MockBehavior.Strict);
            result.Setup(instance => instance.GetResponse()).Returns(response.Object);
            return result.Object;
        }
    }
}