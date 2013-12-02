using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using Moq;
using NUnit.Framework;
using RomanticWeb.DotNetRDF;
using RomanticWeb.Entities;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Ontologies;
using RomanticWeb.Tests.Helpers;
using VDS.RDF;


namespace RomanticWeb.Tests.Linq
{
    [TestFixture]
    public class SparqlTests
    {
        private IEntityContext _entityContext;
        private TripleStore _store;
        private Mock<IClassMapping> _personTypeMappingMock;
        private Mock<IPropertyMapping> _firstNamePropertyMappingMock;
        private Mock<IPropertyMapping> _surnamePropertyMappingMock;
        private Mock<IPropertyMapping> _knowsPropertyMappingMock;
        private Mock<IEntityMapping> _personMappingMock;
        private Mock<IMappingsRepository> _mappingsRepositoryMock;
        private Mock<IOntologyProvider> _ontologyProviderMock;
        private Mock<IEntityContextFactory> _factory;
        private Mock<IGraphSelectionStrategy> _grapheSelectorMock;
        private CompositionContainer _container;

        public interface IPerson:IEntity
        {
            string FirstName { get; }
            string Surname { get; }

            List<IPerson> Knows { get; }
        }

        [SetUp]
        public void Setup()
        {
            _container=new CompositionContainer(new DirectoryCatalog(AppDomain.CurrentDomain.GetPrimaryAssemblyPath()),true);
            _grapheSelectorMock=new Mock<IGraphSelectionStrategy>();
            _grapheSelectorMock.Setup(graphSelector => graphSelector.SelectGraph(It.IsAny<EntityId>())).Returns<EntityId>(entityId => new Uri(entityId.Uri.AbsoluteUri.Replace("magi","data.magi")));
            _store=new TripleStore();
            _store.LoadTestFile("TriplesWithLiteralSubjects.trig");
            _personTypeMappingMock=new Mock<IClassMapping>(MockBehavior.Strict);
            _personTypeMappingMock.SetupGet(typeMapping => typeMapping.Uri).Returns(new Uri("http://xmlns.com/foaf/0.1/Person"));
            _firstNamePropertyMappingMock=new Mock<IPropertyMapping>();
            _firstNamePropertyMappingMock.SetupGet(propertyMapping => propertyMapping.Uri).Returns(new Uri("http://xmlns.com/foaf/0.1/givenName"));
            _firstNamePropertyMappingMock.SetupGet(propertyMapping => propertyMapping.GraphSelector).Returns(_grapheSelectorMock.Object);
            _surnamePropertyMappingMock=new Mock<IPropertyMapping>();
            _surnamePropertyMappingMock.SetupGet(propertyMapping => propertyMapping.Uri).Returns(new Uri("http://xmlns.com/foaf/0.1/familyName"));
            _surnamePropertyMappingMock.SetupGet(propertyMapping => propertyMapping.GraphSelector).Returns(_grapheSelectorMock.Object);
            _knowsPropertyMappingMock=new Mock<IPropertyMapping>();
            _knowsPropertyMappingMock.SetupGet(propertyMapping => propertyMapping.Uri).Returns(new Uri("http://xmlns.com/foaf/0.1/knows"));
            _knowsPropertyMappingMock.SetupGet(propertyMapping => propertyMapping.GraphSelector).Returns(_grapheSelectorMock.Object);
            _personMappingMock=new Mock<IEntityMapping>(MockBehavior.Strict);
            _personMappingMock.SetupGet(mapping => mapping.Classes).Returns(new []{_personTypeMappingMock.Object});
            _personMappingMock.Setup(mapping => mapping.PropertyFor("Surname")).Returns(_surnamePropertyMappingMock.Object);
            _personMappingMock.Setup(mapping => mapping.PropertyFor("FirstName")).Returns(_firstNamePropertyMappingMock.Object);
            _personMappingMock.Setup(mapping => mapping.PropertyFor("Knows")).Returns(_knowsPropertyMappingMock.Object);
            _mappingsRepositoryMock=new Mock<IMappingsRepository>(MockBehavior.Strict);
            _mappingsRepositoryMock.Setup(m => m.RebuildMappings(It.IsAny<MappingContext>()));
            _mappingsRepositoryMock.Setup(repository => repository.MappingFor<IPerson>()).Returns(_personMappingMock.Object);
            _mappingsRepositoryMock.Setup(repository => repository.MappingFor(typeof(IPerson))).Returns(_personMappingMock.Object);
            _ontologyProviderMock=new Mock<IOntologyProvider>(MockBehavior.Strict);
            _ontologyProviderMock.SetupGet(provider => provider.Ontologies).Returns(
                new Ontology[] { new Ontology(
                    new NamespaceSpecification("foaf","http://xmlns.com/foaf/0.1/"),
                    new Class("Person"),
                    new DatatypeProperty("givenName"),
                    new DatatypeProperty("familyName"),
                    new ObjectProperty("knows")),
                new Ontology(
                    new NamespaceSpecification("rdf","http://www.w3.org/1999/02/22-rdf-syntax-ns#"),
                    new Property("type")) });
            _factory=new Mock<IEntityContextFactory>();
            _factory.Setup(factory => factory.SatisfyImports(It.IsNotNull<object>())).Callback<object>(component => { _container.ComposeParts(component); });
            MappingContext mappingContext=new MappingContext(_ontologyProviderMock.Object,new DefaultGraphSelector());
            _entityContext=new EntityContext(_factory.Object,_mappingsRepositoryMock.Object,mappingContext,new EntityStore(),new TripleStoreAdapter(_store));
        }

        [Test]
        [Repeat(5)]
        public void Selecting_entities_by_providing_single_literal_predicate_value_condition_from_pointed_ontology_test()
        {
            IList<IPerson> entities=(from resources in _entityContext.AsQueryable<IPerson>()
                                     where resources.FirstName=="Tomasz"
                                     select resources).ToList();
            Assert.That(entities.Count,Is.EqualTo(1));
            Assert.That(entities[0],Is.Not.Null);
            Assert.That(entities[0],Is.InstanceOf<IPerson>());
            Assert.That(entities[0].FirstName,Is.EqualTo("Tomasz"));
            Assert.That(entities[0].Surname,Is.EqualTo("Pluskiewicz"));
        }

        [Test]
        [Repeat(5)]
        public void Selecting_entities_by_providing_single_literal_predicate_value_condition_test()
        {
            IList<IPerson> entities=(from resources in _entityContext.AsQueryable<IPerson>()
                                     where resources.FirstName=="Tomasz"
                                     select resources).ToList();
            Assert.That(entities.Count,Is.EqualTo(1));
            Assert.That(entities[0],Is.Not.Null);
            Assert.That(entities[0],Is.InstanceOf<IEntity>());
            Assert.That(entities[0].FirstName,Is.EqualTo("Tomasz"));
            Assert.That(entities[0].Surname,Is.EqualTo("Pluskiewicz"));
        }

        [Test]
        [Repeat(5)]
        public void Selecting_entities_by_providing_subject_identifier_condition_test()
        {
            IPerson entity=(from resources in _entityContext.AsQueryable<IPerson>()
                            where resources.Id==(EntityId)"http://magi/people/Tomasz"
                            select resources).FirstOrDefault();
            Assert.That(entity,Is.Not.Null);
            Assert.That(entity,Is.InstanceOf<IEntity>());
            Assert.That(entity.FirstName,Is.EqualTo("Tomasz"));
            Assert.That(entity.Surname,Is.EqualTo("Pluskiewicz"));
        }

        [Test]
        [Repeat(5)]
        public void Selecting_entities_by_providing_entity_mapped_type_condition_test()
        {
            IList<IEntity> entities=(from resources in _entityContext.AsQueryable<IEntity>()
                                     where resources is IPerson
                                     select resources).ToList();
            Assert.That(entities.Count,Is.EqualTo(2));
            IEntity tomasz=entities.Where(item => item.Id==(EntityId)"http://magi/people/Tomasz").FirstOrDefault();
            Assert.That(tomasz,Is.Not.Null);
            Assert.That(tomasz,Is.InstanceOf<Entity>());
            Assert.That(tomasz.AsDynamic().foaf.first_givenName,Is.EqualTo("Tomasz"));
            Assert.That(tomasz.AsDynamic().foaf.first_familyName,Is.EqualTo("Pluskiewicz"));
            IEntity gniewoslaw=entities.Where(item => item.Id==(EntityId)"http://magi/people/Gniewoslaw").FirstOrDefault();
            Assert.That(gniewoslaw,Is.Not.Null);
            Assert.That(gniewoslaw,Is.InstanceOf<Entity>());
            Assert.That(gniewoslaw.AsDynamic().foaf.first_givenName,Is.EqualTo("Gniewosław"));
            Assert.That(gniewoslaw.AsDynamic().foaf.first_familyName,Is.EqualTo("Rzepka"));
        }

        [Test]
        [Repeat(5)]
        public void Selecting_specific_type_entities_test()
        {
            IList<IPerson> entities=(from resources in _entityContext.AsQueryable<IPerson>()
                                     select resources).ToList();
            Assert.That(entities.Count,Is.EqualTo(2));
            IPerson tomasz=entities.Where(item => item.Id==(EntityId)"http://magi/people/Tomasz").FirstOrDefault();
            Assert.That(tomasz,Is.Not.Null);
            Assert.That(tomasz,Is.InstanceOf<IEntity>());
            Assert.That(tomasz.Surname,Is.EqualTo("Pluskiewicz"));
            IPerson gniewoslaw=entities.Where(item => item.Id==(EntityId)"http://magi/people/Gniewoslaw").FirstOrDefault();
            Assert.That(gniewoslaw,Is.Not.Null);
            Assert.That(gniewoslaw,Is.InstanceOf<IEntity>());
            Assert.That(gniewoslaw.Surname,Is.EqualTo("Rzepka"));
        }

        [Test]
        [Repeat(5)]
        public void Selecting_entities_by_providing_nested_predicate_value_condition_test()
        {
            IPerson entity=(from resources in _entityContext.AsQueryable<IPerson>()
                            where resources.Knows.Any(item => item.Id==(EntityId)"http://magi/people/Tomasz")
                            select resources).FirstOrDefault();
            Assert.That(entity,Is.Not.Null);
            Assert.That(entity,Is.InstanceOf<IPerson>());
            Assert.That(entity.FirstName,Is.EqualTo("Gniewosław"));
            Assert.That(entity.Surname,Is.EqualTo("Rzepka"));
        }
    }
}