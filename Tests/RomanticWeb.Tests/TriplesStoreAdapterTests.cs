using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using RomanticWeb.DotNetRDF;
using RomanticWeb.Entities;
using RomanticWeb.Model;
using RomanticWeb.Vocabularies;
using VDS.RDF;
using VDS.RDF.Update;

namespace RomanticWeb.Tests
{
    [TestFixture]
    public class TripleStoreAdapterTests
    {
        private TripleStoreAdapter _tripleStore;
        private Mock<IUpdateableTripleStore> _realStore;
        private IList<EntityQuad> _quadsAdded;
        private IList<EntityQuad> _quadsRemoved;
        private IList<EntityQuad> _entitiesReconstructed;
        private IList<EntityId> _entitiesRemoved;

        [SetUp]
        public void Setup()
        {
            _realStore = new Mock<IUpdateableTripleStore>();
            _quadsAdded = new List<EntityQuad>();
            _quadsRemoved = new List<EntityQuad>();
            _entitiesReconstructed = new List<EntityQuad>();
            _entitiesRemoved = new List<EntityId>();

            _tripleStore = new TripleStoreAdapter(_realStore.Object, null) { MetaGraphUri = new Uri("urn:meta:graph") };
        }

        [Test]
        public void Should_insert_blank_nodes_ins_single_command()
        {
            // given
            var identifier = new EntityId("urn:some:uri");
            var blankNode = new BlankId("magi", identifier);
            var graph = Node.ForUri(new Uri("urn:graph:id"));
            _quadsAdded.Add(new EntityQuad(identifier, Node.FromEntityId(identifier), Node.ForUri(Rdf.predicate), Node.ForLiteral("value"), graph));
            _quadsAdded.Add(new EntityQuad(identifier, Node.FromEntityId(identifier), Node.ForUri(Rdf.predicate), Node.FromEntityId(blankNode), graph));
            _quadsAdded.Add(new EntityQuad(identifier, Node.FromEntityId(identifier), Node.ForUri(Rdf.predicate), Node.FromEntityId(blankNode), graph));
            _quadsAdded.Add(new EntityQuad(blankNode, Node.FromEntityId(blankNode), Node.ForUri(Rdf.predicate), Node.ForLiteral("other value"), graph));
            var changes = new DatasetChanges(_quadsAdded, _quadsRemoved, _entitiesReconstructed, _entitiesRemoved);

            // when
            _tripleStore.Commit();

            // then
            _realStore.Verify(st => st.ExecuteUpdate(It.Is<SparqlUpdateCommandSet>(set => set.CommandCount == 1)));
        }
    }
}