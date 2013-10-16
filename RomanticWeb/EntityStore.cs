using System;
using System.Collections.Generic;
using System.Linq;
using RomanticWeb.Entities;
using RomanticWeb.Model;

namespace RomanticWeb
{
    public class EntityStore:IEntityStore
    {
        private readonly ISet<EntityTriple> _entityQuads; 

        public EntityStore()
        {
            _entityQuads=new SortedSet<EntityTriple>();
        }

        public IEnumerable<EntityTriple> Quads
        {
            get
            {
                return _entityQuads;
            }
        } 

        public IEnumerable<Node> GetObjectsForPredicate(EntityId entityId,Uri predicate)
        {
            return from triple in _entityQuads
                   where triple.Predicate==Node.ForUri(predicate)
                   && triple.Subject==Node.ForUri(entityId.Uri)
                   select triple.Object;
        }

        public void AssertTriple(EntityTriple entityTriple)
        {
            _entityQuads.Add(entityTriple);
        }
    }
}