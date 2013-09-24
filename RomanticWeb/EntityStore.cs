using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using RomanticWeb.Entities;
using RomanticWeb.Ontologies;

namespace RomanticWeb
{
    public class EntityStore:IEntityStore
    {
        private readonly IDictionary<EntityId,QuadCollection> _entityQuads; 

        public EntityStore()
        {
            _entityQuads=new ConcurrentDictionary<EntityId,QuadCollection>();
        }

        public IEnumerable<RdfNode> GetObjectsForPredicate(EntityId entityId,Uri predicate)
        {
            return from triple in _entityQuads.SelectMany(e=>e.Value.Triples)
                   where triple.Predicate==RdfNode.ForUri(predicate)
                         && triple.Subject==RdfNode.ForUri(entityId.Uri)
                   select triple.Object;
        }

        public bool EntityIsCollectionRoot(IEntity potentialListRoot)
        {
            return !(from propertyObjectPair in _entityQuads.SelectMany(e=>e.Value.Triples)
                     where propertyObjectPair.Predicate == RdfNode.ForUri(new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#rest"))
                     && propertyObjectPair.Object == RdfNode.ForUri(potentialListRoot.Id.Uri)
                     select propertyObjectPair).Any();
        }

        public void AssertTriple(EntityId entityId,RdfNode graph,Triple triple)
        {
            if (!_entityQuads.ContainsKey(entityId))
            {
                _entityQuads[entityId]=new QuadCollection();
            }

            _entityQuads[entityId].AssertQuad(graph,triple);
        }
    }
}