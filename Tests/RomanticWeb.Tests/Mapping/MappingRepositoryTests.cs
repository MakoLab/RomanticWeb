using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RomanticWeb.Entities.ResultAggregations;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Conventions;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.Ontologies;
using RomanticWeb.TestEntities;
using RomanticWeb.TestEntities.Animals;

namespace RomanticWeb.Tests.Mapping
{
    [TestFixture]
    public abstract class MappingRepositoryTests
    {
        private MappingsRepository _mappingsRepository;
        private Mock<IOntologyProvider> _ontologies;

        [SetUp]
        public void Setup()
        {
            _ontologies = new Mock<IOntologyProvider>();
            _ontologies.Setup(o => o.ResolveUri(It.IsAny<string>(), It.IsAny<string>()))
                       .Returns((string p, string t) => GetUri(p, t));

            _mappingsRepository=new MappingsRepository();
            _mappingsRepository.AddSource(GetType().Assembly,CreateMappingSource());
            IEnumerable<IConvention> conventions=new IConvention[]
                                                     {
                                                         new DefaultDictionaryKeyPredicateConvention(),
                                                         new DefaultDictionaryValuePredicateConvention(),
                                                         new CollectionConvention(),
                                                         new RdfListConvention()
                                                     };
            _mappingsRepository.RebuildMappings(new MappingContext(_ontologies.Object,conventions));
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
            var mapping=_mappingsRepository.MappingFor<IHuman>();

            // when
            var classMappings=mapping.Classes;

            // then
            classMappings.Should().HaveCount(4);
        }

        [Test]
        public void Mapping_should_allow_dictionary_with_default_key_value()
        {
            // given
            var mapping=_mappingsRepository.MappingFor<IEntityWithDictionary>();

            // then
            var propertyMapping=mapping.PropertyFor("SettingsDefault");
            propertyMapping.Should().BeAssignableTo<IDictionaryMapping>();
            propertyMapping.As<IDictionaryMapping>().KeyPredicate.Should().Be(Vocabularies.Rdf.predicate);
            propertyMapping.As<IDictionaryMapping>().ValuePredicate.Should().Be(Vocabularies.Rdf.@object);
        }

        [TestCase("DefaultListMapping",StoreAs.RdfList)]
        [TestCase("DefaultEnumerableMapping",StoreAs.SimpleCollection)]
        [TestCase("DefaultCollectionMapping",StoreAs.SimpleCollection)]
        public void Default_setting_for_collection_should_be_replaced_by_conventions(string propertyName,StoreAs asExpected)
        {
            // given
            var mapping=_mappingsRepository.MappingFor<IEntityWithCollections>();

            // when
            var property=(ICollectionMapping)mapping.PropertyFor(propertyName);

            // then
            property.StoreAs.Should().Be(asExpected);
        }

        [TestCase("OverridenListMapping",StoreAs.SimpleCollection)]
        [TestCase("OverridenEnumerableMapping",StoreAs.RdfList)]
        [TestCase("OverridenCollectionMapping",StoreAs.RdfList)]
        public void Explicit_setting_for_collection_should_not_be_replaced_by_conventions(string propertyName,StoreAs asExpected)
        {
            // given
            var mapping=_mappingsRepository.MappingFor<IEntityWithCollections>();

            // when
            var property=(ICollectionMapping)mapping.PropertyFor(propertyName);

            // then
            property.StoreAs.Should().Be(asExpected);
        }

        [Test]
        public void Property_should_be_mapped_to_SingleOrDefault_result()
        {
            // given
            var mapping=_mappingsRepository.MappingFor<IAnimal>();

            // when
            var propertyMapping=mapping.PropertyFor("Name");

            // then
            propertyMapping.Aggregation.Should().Be(Aggregation.SingleOrDefault);
        }

        protected abstract IMappingSource CreateMappingSource();

        private static Uri GetUri(string prefix, string term)
        {
            return new Uri("http://example/livingThings#" + term);
        }
    }
}