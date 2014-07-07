using System;
using System.Configuration;
using Moq;
using NUnit.Framework;
using RomanticWeb.DotNetRDF.Configuration;
using VDS.RDF;
using VDS.RDF.Configuration;

namespace RomanticWeb.Tests.Configuration
{
    [TestFixture]
    public class ExternallyConfiguredStoreElementTests
    {
        private ExternallyConfiguredStoreElement _element;
        private Mock<IConfigurationLoader> _loader;

        [SetUp]
        public void Setup()
        {
            _element = new ExternallyConfiguredStoreElement(new StoresConfigurationSection());
            _loader = new Mock<IConfigurationLoader>(MockBehavior.Strict);
            _element.ConfigurationLoader = _loader.Object;
        }

        [TearDown]
        public void Teardown()
        {
            _loader.VerifyAll();
        }

        [Test]
        public void Should_load_store_from_bnode()
        {
            // given
            _element.BlankNodeIdentifier = "store";
            _loader.Setup(l => l.LoadObject<ITripleStore>(It.IsAny<string>())).Returns(new Mock<ITripleStore>().Object);

            // when
            _element.CreateTripleStore();

            // then
            _loader.Verify(l => l.LoadObject<ITripleStore>("store"));
        }

        [Test]
        public void Should_load_store_from_uri()
        {
            // given
            _element.ObjectUri = new Uri("urn:store:uri");
            _loader.Setup(l => l.LoadObject<ITripleStore>(It.IsAny<Uri>())).Returns(new Mock<ITripleStore>().Object);

            // when
            _element.CreateTripleStore();

            // then
            _loader.Verify(l => l.LoadObject<ITripleStore>(new Uri("urn:store:uri")));
        }

        [Test, ExpectedException(typeof(ConfigurationErrorsException))]
        public void Should_throw_when_uri_and_bnode_not_set()
        {
            // given
            // nothing set

            // when
            _element.CreateTripleStore();

            // then
            _loader.Verify(l => l.LoadObject<ITripleStore>(new Uri("urn:store:uri")));
        }

        [Test, ExpectedException(typeof(ConfigurationErrorsException))]
        public void Should_throw_when_both_uri_and_bnode_are_set()
        {
            // given
            _element.ObjectUri = new Uri("urn:store:uri");
            _element.BlankNodeIdentifier = "store";

            // when
            _element.CreateTripleStore();
        }
    }
}