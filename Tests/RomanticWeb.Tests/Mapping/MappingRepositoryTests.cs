using System;
using Moq;
using NUnit.Framework;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Ontologies;
using RomanticWeb.TestEntities.Animals;

namespace RomanticWeb.Tests.Mapping
{
    [TestFixture]
    public abstract class MappingRepositoryTests
    {
        private IMappingsRepository _mappingsRepository;
        private Mock<IOntologyProvider> _ontologies;

        [SetUp]
        public void Setup()
        {
            _ontologies = new Mock<IOntologyProvider>();
            _ontologies.Setup(o => o.ResolveUri(It.IsAny<string>(), It.IsAny<string>()))
                       .Returns((string p, string t) => GetUri(p, t));

            _mappingsRepository = CreateMappingsRepository();
            _mappingsRepository.RebuildMappings(new MappingContext(_ontologies.Object,new DefaultGraphSelector()));
        }

        [Test]
        public void Mapped_type_should_have_mapped_class()
        {
            // given
            var mapping = _mappingsRepository.MappingFor<IAnimal>();

            // when
            var classMapping = mapping.Class;

            // then
            Assert.That(classMapping.Uri, Is.EqualTo(new Uri("http://example/livingThings#Animal")));
        }

        [Test]
        public void Mapped_type_should_contain_declared_properties()
        {
            // given
            var mapping = _mappingsRepository.MappingFor<IAnimal>();

            // when
            var propertyMapping = mapping.PropertyFor("Name");

            // then
            Assert.That(propertyMapping.Name, Is.EqualTo("Name"));
            Assert.That(propertyMapping.Uri, Is.EqualTo(new Uri("http://example/livingThings#name")));
        }

        [Test]
        public void Mapped_type_should_contain_inherited_properties()
        {
            // given
            var mapping = _mappingsRepository.MappingFor<ICarnivore>();

            // when
            var propertyMapping = mapping.PropertyFor("Name");

            // then
            Assert.That(propertyMapping.Name, Is.EqualTo("Name"));
            Assert.That(propertyMapping.Uri, Is.EqualTo(new Uri("http://example/livingThings#name")));
        }

        protected abstract IMappingsRepository CreateMappingsRepository();

        private static Uri GetUri(string prefix, string term)
        {
            return new Uri("http://example/livingThings#" + term);
        }
    }
}