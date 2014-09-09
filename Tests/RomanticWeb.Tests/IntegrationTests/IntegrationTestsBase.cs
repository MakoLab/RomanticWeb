using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using RomanticWeb.DotNetRDF;
using RomanticWeb.LightInject;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Sources;
using RomanticWeb.Ontologies;
using RomanticWeb.TestEntities.Animals;
using RomanticWeb.Tests.Stubs;
using VDS.RDF;

namespace RomanticWeb.Tests.IntegrationTests
{
    public abstract class IntegrationTestsBase
    {
        private IEntityStore _entityStore;
        private IEntityContext _entityContext;
        private IEntityContextFactory _factory;

        public virtual bool IncludeFoaf { get { return false; } }

        public IMappingProviderSource Mappings { get; private set; }

        protected static Uri MetaGraphUri
        {
            get
            {
                return new Uri("http://app.magi/graphs");
            }
        }

        protected IEntityContext EntityContext
        {
            get
            {
                if (_entityContext == null)
                {
                    _entityContext = Factory.CreateContext();
                }

                return _entityContext;
            }
        }

        protected IEntityStore EntityStore { get { return _entityStore; } }

        protected EntityContextFactory Factory { get { return (EntityContextFactory)_factory; } }

        protected abstract ITripleStore Store { get; }

        [SetUp]
        public void Setup()
        {
            Mappings = SetupMappings();

            IServiceContainer container = new ServiceContainer();
            container.Register(factory => Store);
            _factory = new EntityContextFactory(container).WithEntitySource<TripleStoreAdapter>()
                                                 .WithOntology(new DefaultOntologiesProvider())
                                                 .WithOntology(new LifeOntology())
                                                 .WithOntology(new TestOntologyProvider(IncludeFoaf))
                                                 .WithOntology(new ChemOntology())
                                                 .WithMappings(BuildMappings)
                                                 .WithMetaGraphUri(MetaGraphUri)
                                                 .WithDependencies<Components>();

            _entityStore = container.GetInstance<IEntityStore>();

            ChildSetup();
        }

        [TearDown]
        public void Teardown()
        {
            ChildTeardown();
            _entityContext = null;
        }

        protected virtual void BuildMappings(MappingBuilder m)
        {
            m.FromAssemblyOf<IAnimal>();
            m.AddMapping(GetType().Assembly, Mappings);
        }

        protected virtual void ChildTeardown()
        {
        }

        protected virtual IMappingProviderSource SetupMappings()
        {
            var mock = new Mock<IMappingProviderSource>();
            mock.SetupGet(p => p.Description).Returns("Mock provider");
            return mock.Object;
        }

        protected virtual void ChildSetup()
        {
        }

        protected abstract void LoadTestFile(string fileName);

        public class LifeOntology : IOntologyProvider
        {
            public IEnumerable<Ontology> Ontologies
            {
                get
                {
                    yield return new Ontology("life", new Uri("http://example/livingThings#"));
                }
            }

            public Uri ResolveUri(string prefix, string rdfTermName)
            {
                if (prefix == "life")
                {
                    return new Uri("http://example/livingThings#" + rdfTermName);
                }

                return null;
            }
        }

        public class ChemOntology : IOntologyProvider
        {
            public IEnumerable<Ontology> Ontologies
            {
                get
                {
                    yield return new Ontology("chem", new Uri("http://chem.com/vocab/"));
                }
            }

            public Uri ResolveUri(string prefix, string rdfTermName)
            {
                if (prefix == "chem")
                {
                    return new Uri("http://chem.com/vocab/" + rdfTermName);
                }

                return null;
            }
        }
    }
}