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
        private readonly ISet<Tuple<RdfNode,RdfNode,RdfNode,RdfNode>> _entityQuads;

        public EntityStore()
        {
            _entityQuads=new HashSet<Tuple<RdfNode,RdfNode,RdfNode,RdfNode>>();
        }

        public IEnumerable<RdfNode> GetObjectsForPredicate(EntityId entity,Uri predicate)
        {
            return from quad in _entityQuads 
                   where quad.Item2==RdfNode.ForUri(predicate)&&quad.Item1==RdfNode.ForUri(entity.Uri) 
                   select quad.Item3;
        }

        public void AssertTriple(Tuple<RdfNode,RdfNode,RdfNode,RdfNode> entityTriple)
        {
            _entityQuads.Add(entityTriple);
        }

        public bool TripleIsCollectionRoot(IEntity potentialListRoot)
        {
            return !(from quad in _entityQuads
                     where quad.Item2 == RdfNode.ForUri(new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#rest"))
                     && quad.Item3 == RdfNode.ForUri(potentialListRoot.Id.Uri)
                    select quad).Any();
        }
    }
}