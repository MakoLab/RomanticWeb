using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Ontologies;
using RomanticWeb.TestEntities.Inheritance;
using RomanticWeb.Tests.IntegrationTests;
using RomanticWeb.Tests.Stubs;

namespace RomanticWeb.Tests.Mapping
{
    [TestFixture]
    public class PropertyOverridingTests
    {
        private IEntityContextFactory _factory;

        [SetUp]
        protected void Setup()
        {
            _factory = new EntityContextFactory()
                .WithOntology(new DefaultOntologiesProvider())
                .WithOntology(new IntegrationTestsBase.LifeOntology())
                .WithOntology(new TestOntologyProvider(false))
                .WithOntology(new IntegrationTestsBase.ChemOntology())
                .WithMappings(builder => builder.FromAssemblyOf<ISomeEntity>());
        }

        [Test]
        public void Should_override_hidden_property_mapping_in_inherited_interface()
        {
            // Given
            
            // When
            IEntityMapping mapping = _factory.Mappings.MappingFor<ISomeSpecializedEntity>();
            
            // Then
            mapping.Properties.Where(property => property.Name == "Property").Should().HaveCount(1);
        }

        [Test]
        public void Should_override_property_mapping_in_inherited_interface()
        {
            // Given

            // When
            IEntityMapping mapping = _factory.Mappings.MappingFor<ISomeSpecializedEntity>();

            // Then
            mapping.Properties.First(property => property.Name == "Other").Uri.AbsoluteUri.Should().Be("http://other/domain/other");
        }
    }
}