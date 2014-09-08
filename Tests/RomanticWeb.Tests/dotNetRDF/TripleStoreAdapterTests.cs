using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using RomanticWeb.DotNetRDF;
using RomanticWeb.Entities;
using VDS.RDF;
using VDS.RDF.Update;

namespace RomanticWeb.Tests.DotNetRDF
{
    [TestFixture]
    public class TripleStoreAdapterTests
    {
        [Test]
        public void Should_execute_delete_command_for_each_deleted_entity()
        {
            // given
            var tripleStore = new Mock<IUpdateableTripleStore>();
            var tracker = new Mock<IEntityStore>();
            var tripleStoreAdapter = Create(tripleStore, tracker);
            IEnumerable<EntityId> deletedEntities = new[]
                {
                    new EntityId("urn:some:entity1"),
                    new EntityId("urn:some:entity2"),
                    new EntityId("urn:some:entity3"),
                    new EntityId("urn:some:entity4")
                };

            // when
            tripleStoreAdapter.Commit(null);

            // then
            tripleStore.Verify(store => store.ExecuteUpdate(It.Is<SparqlUpdateCommandSet>(set => set.CommandCount == 8)));
        }

        private TripleStoreAdapter Create<TStore>(Mock<TStore> store, Mock<IEntityStore> tracker) where TStore : class, ITripleStore
        {
            var tripleStoreAdapter = new TripleStoreAdapter(store.Object, tracker.Object, null)
                                       {
                                           MetaGraphUri = new Uri("http://app.magi/graphs")
                                       };
            var metagraph = new Graph { BaseUri = tripleStoreAdapter.MetaGraphUri };
            store.Setup(s => s.HasGraph(tripleStoreAdapter.MetaGraphUri)).Returns(true);
            store.Setup(s => s[tripleStoreAdapter.MetaGraphUri]).Returns(metagraph);
            return tripleStoreAdapter;
        }
    }
}