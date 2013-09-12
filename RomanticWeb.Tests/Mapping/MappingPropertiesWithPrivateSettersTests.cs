using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using RomanticWeb.Ontologies;
using RomanticWeb.Tests.Helpers;
using RomanticWeb.Tests.Stubs;
using RomanticWeb.dotNetRDF;
using VDS.RDF;

namespace RomanticWeb.Tests.Mapping
{
    [TestFixture]
    public class MappingPropertiesWithPrivateSettersTests
    {
        private IPerson _entity;
        private TripleStore _tripleStore;
        private Mock<IMapping<IPerson>> _personMapping;
        private Mock<IMappingProvider> _mappings;
        private Ontology _foaf;

        [SetUp]
        public void Setup()
        {
            _tripleStore = new TripleStore();
            _mappings = new Mock<IMappingProvider>();
            _personMapping = new Mock<IMapping<IPerson>>();
            _mappings.Setup(m => m.MappingFor<IPerson>()).Returns(_personMapping.Object);
            var staticOntologyProvider = new StaticOntologyProvider();
            _foaf = staticOntologyProvider.Ontologies.First();
            var factory = new dotNetRDF.EntityFactory(_tripleStore, _mappings.Object, staticOntologyProvider);
            _entity = factory.Create<IPerson>(new UriId("http://magi/people/Tomasz"));
        }

        [TearDown]
        public void Teardown()
        {
            _personMapping.VerifyAll();
            _mappings.VerifyAll();
        }

        [Test]
        public void Should_return_actual_value_from_rdf_data()
        {
            // given
            _tripleStore.LoadTestFile("TriplesWithLiteralSubjects.ttl");
            // todo refactor Property so that it can be used with Uris
            _personMapping.Setup(m => m.PropertyFor("FirstName")).Returns(new Property("givenName").InOntology(_foaf));

            // when
            string firstName = _entity.FirstName;

            // then
            Assert.That(firstName, Is.EqualTo("Tomasz"));
        }

        public interface IPerson
        {
            string FirstName { get; }
        }
    }
}
