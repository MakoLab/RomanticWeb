using System;
using System.Collections.Generic;
using System.Linq;
using Anotar.NLog;
using NullGuard;
using RomanticWeb.Entities;
using RomanticWeb.Model;
using RomanticWeb.Updates;

namespace RomanticWeb
{
    internal class EntityStore : IEntityStore
    {
        private readonly IDatasetChangesTracker _changesTracker;
        private readonly IEntityQuadCollection _entityQuads;
        private readonly IEntityQuadCollection _initialQuads;

        public EntityStore(IDatasetChangesTracker changesTracker)
        {
            _changesTracker = changesTracker;
            _entityQuads = new EntityQuadCollection2();
            _initialQuads = new EntityQuadCollection2();
        }

        public IEnumerable<EntityQuad> Quads { get { return _entityQuads; } }

        public IDatasetChanges Changes
        {
            get
            {
                return _changesTracker;
            }
        }

        public IEnumerable<Node> GetObjectsForPredicate(EntityId entityId, Uri predicate, [AllowNull] Uri graph)
        {
            var quads = from triple in _entityQuads.GetEntityQuads(entityId)
                        where triple.Predicate.Uri.AbsoluteUri == predicate.AbsoluteUri
                        select triple;

            if (graph != null)
            {
                quads = quads.Where(triple => GraphEquals(triple, graph));
            }

            return quads.Select(triple => triple.Object).ToList();
        }

        public IEnumerable<EntityQuad> GetEntityQuads(EntityId entityId, bool includeBlankNodes = true)
        {
            throw new NotImplementedException();
        }

        public void AssertEntity(EntityId entityId, IEnumerable<EntityQuad> entityTriples)
        {
            if (_entityQuads.Entities.Contains(entityId))
            {
                LogTo.Info("Skipping entity {0}. Entity already added to store", entityId);
                return;
            }

            var entityQuads = entityTriples as EntityQuad[] ?? entityTriples.ToArray();
            _entityQuads.Add(entityId, entityQuads);
            _initialQuads.Add(entityId, entityQuads);
        }

        public void ReplacePredicateValues(EntityId entityId, Node propertyUri, Func<IEnumerable<Node>> newValues, Uri graphUri)
        {
            EntityQuad[] newQuads;
            EntityQuad[] removedQuads;
            var subjectNode = Node.FromEntityId(entityId);

            ////if ((propertyUri.IsUri) && (propertyUri.Uri.AbsoluteUri == Rdf.type.AbsoluteUri))
            ////{
            ////    removedQuads = _entityQuads.GetEntityTypeQuads(entityId).ToArray();
            ////    newQuads = (from node in newValues()
            ////               select new EntityQuad(entityId, subjectNode, propertyUri, node).InGraph(graphUri)).ToArray();

            ////    _entityQuads.SetEntityTypeQuads(entityId, newQuads, graphUri);
            ////}
            ////else
            {
                removedQuads = RemoveTriples(entityId, subjectNode, propertyUri, graphUri).ToArray();

                newQuads = (from node in newValues()
                           select new EntityQuad(entityId, subjectNode, propertyUri, node).InGraph(graphUri)).ToArray();

                foreach (var triple in newQuads)
                {
                    _entityQuads.Add(triple);
                }
            }

            _changesTracker.Add(new GraphUpdate(entityId, graphUri, removedQuads, newQuads));
        }

        public void Delete(EntityId entityId, DeleteBehaviour deleteBehaviour = DeleteBehaviour.Default)
        {
            var deletes = from entityQuad in _entityQuads.GetEntityQuads(entityId).ToList()
                          from removedQuad in RemoveTriple(entityQuad)
                          select removedQuad;

            if (entityId is BlankId)
            {
                deletes = deletes.Union(_entityQuads.RemoveWhereObject(entityId));
            }

            var deletesGrouped = from removedQuad in deletes 
                                 group removedQuad by removedQuad.Graph into g 
                                 select g;

            TrackChanges(entityId, deleteBehaviour, deletesGrouped);
        }

        public void ResetState()
        {
            _initialQuads.Clear();
            foreach (var entityId in _entityQuads.Entities)
            {
                _initialQuads.Add(entityId, _entityQuads.GetEntityQuads(entityId));
            }

            _changesTracker.Clear();
        }

        /// <summary>
        /// Removes triple and blank node's subgraph if present
        /// </summary>
        /// <returns>a value indicating that the was a blank node object value</returns>
        private IEnumerable<EntityQuad> RemoveTriples(EntityId entityId, Node subjectNode, Node propertyUri = null, Uri graphUri = null)
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

            return quadsRemoved.ToList().SelectMany(RemoveTriple);
        }

        private IEnumerable<EntityQuad> RemoveTriple(EntityQuad entityTriple)
        {
            _entityQuads.Remove(entityTriple);

            if (entityTriple.Object.IsBlank)
            {
                foreach (var removedQuad in RemoveTriples(entityTriple.EntityId, entityTriple.Object))
                {
                    yield return removedQuad;
                }
            }

            yield return entityTriple;
        }

        // TODO: Make the GraphEquals method a bit less rigid.
        private bool GraphEquals(EntityQuad triple, Uri graph)
        {
            return (triple.Graph.Uri.AbsoluteUri == graph.AbsoluteUri) || ((triple.Subject.IsBlank) && (graph.AbsoluteUri.EndsWith(triple.Graph.Uri.AbsoluteUri)));
        }

        private void TrackChanges(EntityId entityId, DeleteBehaviour deleteBehaviour, IEnumerable<IGrouping<Node, EntityQuad>> deletesGrouped)
        {
            foreach (var removed in deletesGrouped.ToList())
            {
                if (removed.Any(quad => quad.Object.IsBlank || quad.Subject.IsBlank))
                {
                    var removedQuads = removed;
                    _changesTracker.Add(new GraphReconstruct(entityId, removed.Key.ToEntityId(), Quads.Where(q => q.Graph == removedQuads.Key)));
                }
                else
                {
                    _changesTracker.Add(new GraphDelete(entityId, removed.Key.Uri));
                }
            }

            if (!(entityId is BlankId) && deleteBehaviour.HasFlag(DeleteBehaviour.NullifyChildren))
            {
                _entityQuads.RemoveWhereObject(entityId);
                _changesTracker.Add(new RemoveReferences(entityId));
            }
        }
    }
}