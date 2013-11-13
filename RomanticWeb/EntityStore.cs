using System;
using System.Collections.Generic;
using System.Linq;
using Anotar.NLog;
using NullGuard;
using RomanticWeb.Entities;
using RomanticWeb.Model;

namespace RomanticWeb
{
    internal class EntityStore:IEntityStore
    {
        private readonly ISet<EntityTriple> _entityQuads;

        private readonly ISet<EntityTriple> _removedTriples;

        private readonly ISet<EntityTriple> _addedTriples; 

        public EntityStore()
        {
            _entityQuads=new SortedSet<EntityTriple>();
            _removedTriples=new HashSet<EntityTriple>();
            _addedTriples=new HashSet<EntityTriple>();
        }

        public IEnumerable<EntityTriple> Quads
        {
            get
            {
                return _entityQuads;
            }
        }

        public DatasetChanges Changes
        {
            get
            {
                return new DatasetChanges(_addedTriples,_removedTriples);
            }
        }

        public IEnumerable<Node> GetObjectsForPredicate(EntityId entityId,Uri predicate,[AllowNull] Uri graph)
        {
            var quads=from triple in _entityQuads
                      where triple.Predicate==Node.ForUri(predicate)
                         && triple.Subject==Node.ForUri(entityId.Uri)
                      select triple;

            if (graph!=null)
            {
                quads=quads.Where(triple => triple.Graph==Node.ForUri(graph));
            }

            return quads.Select(triple => triple.Object);
        }

        public void AssertEntity(EntityId entityId,IEnumerable<EntityTriple> entityTriples)
        {
            if (_entityQuads.Any(quad => quad.EntityId==entityId))
            {
                LogTo.Info("Skipping entity {0}. Entity already added to store", entityId);
                return;
            }

            foreach (var triple in entityTriples)
            {
                if (triple.EntityId!=entityId)
                {
                    throw new ArgumentException(string.Format("All EntityTriples must reference EntityId {0} but {1} found",entityId,triple.EntityId), "entityTriples");
                }

                _entityQuads.Add(triple);
            }
        }

        public void ReplacePredicateValues(EntityId entityId,Node propertyUri,IEnumerable<Node> valueNodes,Uri graphUri)
        {
            var subjectNode=Node.ForUri(entityId.Uri);
            var quadsRemoved = from quad in Quads
                               where quad.EntityId == entityId
                               && quad.Predicate == propertyUri
                               && quad.Subject == subjectNode
                               select quad;

            if (graphUri != null)
            {
                quadsRemoved = quadsRemoved.Where(quad => quad.Graph == Node.ForUri(graphUri));
            }

            foreach (var entityTriple in quadsRemoved.ToList())
            {
                _entityQuads.Remove(entityTriple);
                _removedTriples.Add(entityTriple);
            }

            foreach (var valueNode in valueNodes)
            {
                var triple=new EntityTriple(entityId,subjectNode,propertyUri,valueNode).InGraph(graphUri);
                _entityQuads.Add(triple);
                _addedTriples.Add(triple);
            }
        }
    }
}