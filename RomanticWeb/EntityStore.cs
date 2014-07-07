using System;
using System.Collections.Generic;
using System.Linq;
using Anotar.NLog;
using NullGuard;
using RomanticWeb.Entities;
using RomanticWeb.Model;
using RomanticWeb.Vocabularies;

namespace RomanticWeb
{
    internal class EntityStore : IEntityStore
    {
        private readonly EntityQuadCollection _entityQuads;
        private readonly EntityQuadCollection _initialQuads;
        private readonly IDictionary<EntityId, DeleteBehaviours> _deletedEntities;
        private IDictionary<EntityId, DeleteBehaviours> _markedForDeletion;

        public EntityStore()
        {
            _entityQuads = new EntityQuadCollection();
            _initialQuads = new EntityQuadCollection();
            _deletedEntities = new Dictionary<EntityId, DeleteBehaviours>();
        }

        public IEnumerable<EntityQuad> Quads { get { return _entityQuads.Quads; } }

        public DatasetChanges Changes
        {
            get
            {
                DatasetChangesGenerator datasetChangesGenerator = new DatasetChangesGenerator(_initialQuads, _entityQuads, _deletedEntities);
                DatasetChanges result = datasetChangesGenerator.GenerateDatasetChanges();
                _markedForDeletion = datasetChangesGenerator.MarkedForDeletion;
                return result;
            }
        }

        public IEnumerable<Node> GetObjectsForPredicate(EntityId entityId, Uri predicate, [AllowNull] Uri graph)
        {
            IEnumerable<EntityQuad> quads;
            if (predicate.AbsoluteUri == Rdf.type.AbsoluteUri)
            {
                quads = _entityQuads.GetEntityTypeQuads(entityId);
            }
            else
            {
                quads = from triple in _entityQuads[entityId]
                        where triple.Predicate.Uri.AbsoluteUri == predicate.AbsoluteUri
                        select triple;
            }

            if (graph != null)
            {
                quads = quads.Where(triple => GraphEquals(triple, graph));
            }

            return quads.Select(triple => triple.Object).ToList();
        }

        public IEnumerable<EntityQuad> GetEntityQuads(EntityId entityId, bool includeBlankNodes = true)
        {
            return (includeBlankNodes ? _entityQuads.GetEntityQuads(entityId) : _entityQuads[entityId]);
        }

        public void AssertEntity(EntityId entityId, IEnumerable<EntityQuad> entityTriples)
        {
            _markedForDeletion = null;
            if (_entityQuads.Entities.Contains(entityId))
            {
                LogTo.Info("Skipping entity {0}. Entity already added to store", entityId);
                return;
            }

            _entityQuads.Add(entityId, entityTriples);
            _initialQuads.Add(entityId, entityTriples);
        }

        public void ReplacePredicateValues(EntityId entityId, Node propertyUri, Func<IEnumerable<Node>> getNewValues, Uri graphUri)
        {
            _markedForDeletion = null;
            var subjectNode = Node.FromEntityId(entityId);
            RemoveTriples(entityId, subjectNode, propertyUri, graphUri);

            foreach (var valueNode in getNewValues())
            {
                var triple = new EntityQuad(entityId, subjectNode, propertyUri, valueNode).InGraph(graphUri);
                _entityQuads.Add(triple);
            }
        }

        public void Delete(EntityId entityId, DeleteBehaviours deleteBehaviour = DeleteBehaviours.DeleteVolatileChildren | DeleteBehaviours.NullifyVolatileChildren)
        {
            _markedForDeletion = null;
            _deletedEntities[entityId] = deleteBehaviour;
        }

        public void ResetState()
        {
            foreach (var deleted in _markedForDeletion ?? new DatasetChangesGenerator(_initialQuads, _entityQuads, _deletedEntities).MarkedForDeletion)
            {
                _entityQuads.Remove(deleted.Key);
                if ((((deleted.Value & DeleteBehaviours.NullifyVolatileChildren) == DeleteBehaviours.NullifyVolatileChildren) && (deleted.Key is BlankId)) ||
                    ((deleted.Value & DeleteBehaviours.NullifyChildren) == DeleteBehaviours.NullifyChildren))
                {
                    _entityQuads.RemoveWhereObject(deleted.Key);
                }
            }

            _initialQuads.Clear();
            foreach (EntityId entityId in _entityQuads)
            {
                _initialQuads.Add(entityId, _entityQuads[entityId]);
            }

            _markedForDeletion = null;
        }

        private void RemoveTriples(EntityId entityId, Node subjectNode, Node propertyUri = null, Uri graphUri = null)
        {
            var quadsRemoved = from quad in Quads
                               where quad.EntityId == entityId && quad.Subject == subjectNode
                               select quad;

            if (propertyUri != null)
            {
                quadsRemoved = quadsRemoved.Where(quad => quad.Predicate == propertyUri);
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

            if (entityTriple.Object.IsBlank)
            {
                RemoveTriples(entityTriple.EntityId, entityTriple.Object);
            }
        }

        // TODO: Make the GraphEquals method a bit less rigid.
        private bool GraphEquals(EntityQuad triple, Uri graph)
        {
            return (triple.Graph.Uri.AbsoluteUri == graph.AbsoluteUri) || ((triple.Subject.IsBlank) && (graph.AbsoluteUri.EndsWith(triple.Graph.Uri.AbsoluteUri)));
        }
    }
}