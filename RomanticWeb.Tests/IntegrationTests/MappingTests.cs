using System;
using Moq;
using NUnit.Framework;
using RomanticWeb.Tests.Stubs;

namespace RomanticWeb.Tests.IntegrationTests
{
    [TestFixture]
    public class MappingTests : InMemoryTripleStoreTestsBase
    {
        private IPerson _entity;
        private Mock<IMapping<IPerson>> _personMapping;

        protected override void ChildSetup()
        {
            _entity = EntityFactory.Create<IPerson>(new UriId("http://magi/people/Tomasz"));
        }

        protected override Mock<IMappingProvider> SetupMappings()
        {
            var mappings = new Mock<IMappingProvider>();
            _personMapping = new Mock<IMapping<IPerson>>();
            mappings.Setup(m => m.MappingFor<IPerson>()).Returns(_personMapping.Object);
            return mappings;
        }

        protected override void ChildTeardown()
        {
            _personMapping.VerifyAll();
        }

        [Test]
        public void Should_return_actual_value_from_rdf_data()
        {
            // given
            LoadTestFile("TriplesWithLiteralSubjects.ttl");
            // todo refactor Property so that it can be used with Uris
            _personMapping.Setup(m => m.PropertyFor("FirstName")).Returns(new TestPropertyMapping
                {
                    Uri = new Uri("http://xmlns.com/foaf/0.1/givenName")
                });

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
