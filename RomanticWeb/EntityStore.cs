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
        private readonly ISet<EntityId> _deletedEntites; 
        private readonly ISet<EntityQuad> _entityQuads;
        private readonly ISet<EntityQuad> _removedTriples;
        private readonly ISet<EntityQuad> _addedTriples;
        private readonly ISet<Tuple<Uri,EntityId>> _metaGraphChanges; 

        public EntityStore()
        {
            _entityQuads=new SortedSet<EntityQuad>();
            _removedTriples=new HashSet<EntityQuad>();
            _addedTriples=new HashSet<EntityQuad>();
            _metaGraphChanges=new HashSet<Tuple<Uri,EntityId>>();
            _deletedEntites=new HashSet<EntityId>();
        }

        public IEnumerable<EntityQuad> Quads
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
                return new DatasetChanges(_addedTriples, _removedTriples, _metaGraphChanges, _deletedEntites);
            }
        }

        public IEnumerable<Node> GetObjectsForPredicate(EntityId entityId,Uri predicate,[AllowNull] Uri graph)
        {
            var quads=from triple in _entityQuads
                      where triple.Predicate==Node.ForUri(predicate)
                         &&triple.Subject==(entityId is BlankId?Node.ForBlank(((BlankId)entityId).Identifier,((BlankId)entityId).RootEntityId,((BlankId)entityId).Graph):Node.ForUri(entityId.Uri))
                      select triple;

            if (graph!=null)
            {
                quads=quads.Where(triple => GraphEquals(triple,graph));
            }

            return quads.Select(triple => triple.Object).ToList();
        }

        public void AssertEntity(EntityId entityId,IEnumerable<EntityQuad> entityTriples)
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

        public void ReplacePredicateValues(EntityId entityId,Node propertyUri,Func<IEnumerable<Node>> getNewValues,Uri graphUri)
        {
            var subjectNode=Node.FromEntityId(entityId);
            RemoveTriples(entityId,subjectNode,propertyUri,graphUri);

            foreach (var valueNode in getNewValues())
            {
                var triple=new EntityQuad(entityId,subjectNode,propertyUri,valueNode).InGraph(graphUri);
                _entityQuads.Add(triple);
                _addedTriples.Add(triple);
                _metaGraphChanges.Add(Tuple.Create(graphUri,entityId));
            }
        }

        public void Delete(EntityId entityId)
        {
            _deletedEntites.Add(entityId);
        }

        private void RemoveTriples(EntityId entityId, Node subjectNode, Node propertyUri=null, Uri graphUri=null)
        {
            var quadsRemoved = from quad in Quads
                               where quad.EntityId == entityId && quad.Subject == subjectNode
                               select quad;

            if (propertyUri!=null)
            {
                quadsRemoved=quadsRemoved.Where(quad => quad.Predicate==propertyUri);
            }

            if (graphUri != null)
            {
                quadsRemoved = quadsRemoved.Where(quad => GraphEquals(quad, graphUri));
            }

            foreach (var entityTriple in quadsRemoved.ToList())
            {
                RemoveTriple(entityTriple);
            }
        }

        private void RemoveTriple(EntityQuad entityTriple)
        {
            _entityQuads.Remove(entityTriple);
            _removedTriples.Add(entityTriple);

            if (entityTriple.Object.IsBlank)
            {
                RemoveTriples(entityTriple.EntityId, entityTriple.Object);
            }
        }

        private bool GraphEquals(EntityQuad triple,Uri graph)
        {
            return (triple.Graph.Uri.AbsoluteUri==graph.AbsoluteUri)||((triple.Subject.IsBlank)&&(graph.AbsoluteUri.EndsWith(triple.Graph.Uri.AbsoluteUri)));
        }
    }
}