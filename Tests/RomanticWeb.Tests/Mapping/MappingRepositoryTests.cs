using System;
using System.Linq;
using FluentAssertions;
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
            var classMapping = mapping.Classes.First();

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
        public void Type_can_be_mapped_without_rdf_type()
        {
            // given
            var mapping = _mappingsRepository.MappingFor<IUntypedAnimal>();

            // then
            Assert.That(mapping.Classes,Is.Empty);
            Assert.That(mapping.PropertyFor("Name"), Is.Not.Null);
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

        [Test]
        public void Subclass_should_inherit_parent_Class_mappings()
        {
            // given
            var mapping = _mappingsRepository.MappingFor<IHuman>();

            // when
            var classMappings=mapping.Classes;

            // then
            classMappings.Should().HaveCount(4);
        }

        protected abstract IMappingsRepository CreateMappingsRepository();

        private static Uri GetUri(string prefix, string term)
        {
            return new Uri("http://example/livingThings#" + term);
        }
    }
}