using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Sources;
using RomanticWeb.Ontologies;
using RomanticWeb.TestEntities.Animals;
using RomanticWeb.Tests.Stubs;

namespace RomanticWeb.Tests.IntegrationTests
{
    public abstract class IntegrationTestsBase
    {
        private EntityStore _entityStore;
        private IEntityContext _entityContext;
        private IEntityContextFactory _factory;

        public virtual bool IncludeFoaf
        {
            get
            {
                return false;
            }
        }

        public IMappingProviderSource Mappings { get; private set; }

        protected IEntityContext EntityContext
        {
            get
            {
                if (_entityContext==null)
                {
                    _entityContext= Factory.CreateContext();
                }

                return _entityContext;
            }
        }

        protected IEntityStore EntityStore
        {
            get
            {
                return _entityStore;
            }
        }

        protected EntityContextFactory Factory
        {
            get
            {
                return (EntityContextFactory)_factory;
            }
        }

        [SetUp]
        public void Setup()
        {
            Mappings=SetupMappings();
            _entityStore=new EntityStore();

            _factory=new EntityContextFactory().WithEntitySource(CreateEntitySource)
                                               .WithOntology(new DefaultOntologiesProvider())
                                               .WithOntology(new LifeOntology())
                                               .WithOntology(new TestOntologyProvider(IncludeFoaf))
                                               .WithMappings(BuildMappings)
                                               .WithEntityStore(() => _entityStore);
            ChildSetup();
        }

        [TearDown]
        public void Teardown() 
        {
            ChildTeardown();
            _entityContext=null;
        }

        protected virtual void BuildMappings(MappingBuilder m)
        {
            m.FromAssemblyOf<IAnimal>();
            m.AddMapping(GetType().Assembly,Mappings);
        }

        protected virtual void ChildTeardown() 
        {
        }

        protected virtual IMappingProviderSource SetupMappings()
        {
            return new Mock<IMappingProviderSource>().Object;
        }

        protected virtual void ChildSetup() 
        {
        }

        protected abstract void LoadTestFile(string fileName); 
         
        protected abstract IEntitySource CreateEntitySource();

        public class LifeOntology : IOntologyProvider
        {
            public IEnumerable<Ontology> Ontologies
            {
                get
                {
                    yield return new Ontology("life", "http://example/livingThings#");
                }
            }

            public Uri ResolveUri(string prefix,string rdfTermName)
            {
                if (prefix=="life")
                {
                    return new Uri("http://example/livingThings#"+rdfTermName);
                }
             
                return null;
            }
        }
    }
}