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
        private readonly ISet<EntityTriple> _entityQuads; 

        public EntityStore()
        {
            _entityQuads=new SortedSet<EntityTriple>();
        }

        public IEnumerable<Node> GetObjectsForPredicate(EntityId entityId,Uri predicate)
        {
            return from triple in _entityQuads
                   where triple.Predicate==Node.ForUri(predicate)
                   && triple.Subject==Node.ForUri(entityId.Uri)
                   select triple.Object;
        }

        public bool EntityIsCollectionRoot(IEntity potentialListRoot)
        {
            // todo: check negative scenario for false positive
            return !(from propertyObjectPair in _entityQuads
                     where propertyObjectPair.Predicate == Node.ForUri(new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#rest"))
                     && propertyObjectPair.Object == Node.ForUri(potentialListRoot.Id.Uri)
                     select propertyObjectPair).Any();
        }

        public void AssertTriple(EntityTriple entityTriple)
        {
            _entityQuads.Add(entityTriple);
        }
    }
}