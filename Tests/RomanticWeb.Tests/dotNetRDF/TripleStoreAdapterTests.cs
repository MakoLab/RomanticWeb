using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using RomanticWeb.DotNetRDF;
using RomanticWeb.Updates;
using VDS.RDF;
using VDS.RDF.Update;

namespace RomanticWeb.Tests.DotNetRDF
{
    [TestFixture]
    public class TripleStoreAdapterTests
    {
        private TripleStoreAdapter _tripleStore;
        private Mock<IUpdateableTripleStore> _realStore;
        private Mock<IEntityStore> _entityStore;
        private Mock<ISparqlCommandFactory> _factory;

        [SetUp]
        public void Setup()
        {
            _realStore = new Mock<IUpdateableTripleStore>();
            _entityStore = new Mock<IEntityStore>(MockBehavior.Strict);
            _factory = new Mock<ISparqlCommandFactory>(MockBehavior.Strict);

            _tripleStore = new TripleStoreAdapter(_realStore.Object, _entityStore.Object, _factory.Object) { MetaGraphUri = new Uri("urn:meta:graph") };
        }

        [Test]
        public void Should_convert_commands_and_execute_on_triple_store()
        {
            // given
            var testCommand = new TestCommand();
            var testUpdates = new[] { new TestUpdate() };
            _factory.Setup(f => f.CreateCommandSet(testUpdates))
                    .Returns(new[] { testCommand });

            // when
            _tripleStore.Commit(testUpdates);

            // then
            _realStore.Verify(st => st.ExecuteUpdate(It.Is<SparqlUpdateCommandSet>(set => set.Commands.Single() == testCommand)));
        }

        public class TestCommand : SparqlUpdateCommand
        {
            public TestCommand()
                : base(SparqlUpdateCommandType.Add)
            {
            }

            public override bool AffectsSingleGraph
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override bool AffectsGraph(Uri graphUri)
            {
                throw new NotImplementedException();
            }

            public override void Evaluate(SparqlUpdateEvaluationContext context)
            {
                throw new NotImplementedException();
            }

            public override void Process(ISparqlUpdateProcessor processor)
            {
                throw new NotImplementedException();
            }

            public override string ToString()
            {
                throw new NotImplementedException();
            }
        }

        public class TestUpdate : DatasetChange
        {
            public TestUpdate()
                : base("urn:test:entity", "urn:test:graph")
            {
            }
        }
    }
}