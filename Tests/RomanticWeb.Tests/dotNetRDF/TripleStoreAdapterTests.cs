using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using RomanticWeb.DotNetRDF;
using RomanticWeb.Entities;
using RomanticWeb.Model;
using VDS.RDF;
using VDS.RDF.Update;

namespace RomanticWeb.Tests.dotNetRDF
{
    [TestFixture]
    public class TripleStoreAdapterTests
    {
        [Test]
        public void Should_execute_delete_command_for_each_deleted_entity()
        {
            // given
            var tripleStore=new Mock<IUpdateableTripleStore>();
            var tripleStoreAdapter=Create(tripleStore);
            IEnumerable<EntityId> deletedEntities=new[]
                {
                    new EntityId("urn:some:entity1"),
                    new EntityId("urn:some:entity2"),
                    new EntityId("urn:some:entity3"),
                    new EntityId("urn:some:entity4")
                };
            var changes=new DatasetChanges(new EntityQuad[0],new EntityQuad[0],new EntityQuad[0],deletedEntities);

            // when
            tripleStoreAdapter.ApplyChanges(changes);

            // then
            tripleStore.Verify(store => store.ExecuteUpdate(It.Is<SparqlUpdateCommandSet>(set => set.CommandCount==8)));
        }

        private TripleStoreAdapter Create<TStore>(Mock<TStore> store) where TStore:class,ITripleStore
        {
            var tripleStoreAdapter=new TripleStoreAdapter(store.Object)
                                       {
                                           MetaGraphUri=new Uri("http://app.magi/graphs")
                                       };
            var metagraph=new Graph { BaseUri=tripleStoreAdapter.MetaGraphUri };
            store.Setup(s => s.HasGraph(tripleStoreAdapter.MetaGraphUri)).Returns(true);
            store.Setup(s => s[tripleStoreAdapter.MetaGraphUri]).Returns(metagraph);
            return tripleStoreAdapter;
        }
    }
}