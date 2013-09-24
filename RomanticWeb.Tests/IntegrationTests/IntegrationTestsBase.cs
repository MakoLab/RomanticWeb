using System;
using Moq;
using NUnit.Framework;
using RomanticWeb.Mapping;
using RomanticWeb.Tests.Stubs;

namespace RomanticWeb.Tests.IntegrationTests
{
    public abstract class IntegrationTestsBase
    {
        private IMappingsRepository _mappings;

        public IMappingsRepository Mappings
        {
            get
            {
                return _mappings;
            }
        }

        protected IEntityFactory EntityFactory { get; private set; }

        [SetUp]
        public void Setup()
        {
            _mappings = SetupMappings();
            EntityFactory = new EntityContext(Mappings, new TestOntologyProvider(), CreateEntitySource());

            ChildSetup();
        }

        [TearDown]
        public void Teardown()
        {
            ChildTeardown();
        }

        protected virtual void ChildTeardown()
        {
        }

        protected virtual IMappingsRepository SetupMappings()
        {
            return new Mock<IMappingsRepository>(MockBehavior.Strict).Object;
        }

        protected virtual void ChildSetup()
        {
        }

        protected abstract void LoadTestFile(string fileName);

        protected abstract void LoadTestFile(string fileName, Uri graphUri);

        protected abstract IEntitySource CreateEntitySource();
    }
}