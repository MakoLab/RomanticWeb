using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using RomanticWeb.Entities;
using RomanticWeb.Model;

namespace RomanticWeb.Updates
{
    /// <summary>Represents changes made in the triple store.</summary>
    public sealed class DatasetChanges : IDatasetChangesTracker
    {
        private const int GraphChangesCapacity = 16;

        private readonly IDictionary<EntityId, IList<DatasetChange>> _graphChanges = new ConcurrentDictionary<EntityId, IList<DatasetChange>>();

        public DatasetChanges()
            : this(new EntityQuad[0], new EntityQuad[0], new EntityQuad[0], new EntityId[0])
        {
        }

        internal DatasetChanges(
            IEnumerable<EntityQuad> quadsAdded,
            IEnumerable<EntityQuad> quadsRemoved,
            IEnumerable<EntityQuad> entitiesReconstructed,
            IEnumerable<EntityId> entitiesRemoved)
        {
            QuadsAdded = quadsAdded;
            QuadsRemoved = quadsRemoved;
            EntitiesReconstructed = entitiesReconstructed;
            EntitiesRemoved = entitiesRemoved;
        }

        /// <summary>Gets the added quads.</summary>
        /// <remarks>Implementeurs of the <see cref="IEntitySource" /> are supposed to insert these statements after quads are removed.</remarks>
        [Obsolete]
        public IEnumerable<EntityQuad> QuadsAdded { get; private set; }

        /// <summary>Gets the quads removed.</summary>
        /// <remarks>Implementeurs of the <see cref="IEntitySource" /> are to delete these statements first.</remarks>
        [Obsolete]
        public IEnumerable<EntityQuad> QuadsRemoved { get; private set; }

        /// <summary>Gets the entity graphs to be reconstructed.</summary>
        /// <remarks>Implementeurs of the <see cref="IEntitySource" /> are supposed to delete whole graph of a given entity, and then to insert all given statements.</remarks>
        [Obsolete]
        public IEnumerable<EntityQuad> EntitiesReconstructed { get; private set; }

        /// <summary>Gets the entities to be removed.</summary>
        /// <remarks>Implementeurs of the <see cref="IEntitySource" /> are supposed to delete whole graph of a given entity.</remarks>
        [Obsolete]
        public IEnumerable<EntityId> EntitiesRemoved { get; private set; }

        /// <summary>Gets a value indicating whether there are any changes.</summary>
        public bool HasChanges
        {
            get
            {
                return _graphChanges.Any() && _graphChanges.All(changes => changes.Value.Any());
            }
        }

        public IEnumerable<DatasetChange> this[EntityId graphUri]
        {
            get
            {
                return _graphChanges[graphUri];
            }
        }

        public void Add(DatasetChange datasetChange)
        {
            ChangesFor(datasetChange.Graph).Add(datasetChange);
        }

        public IEnumerator<KeyValuePair<EntityId, IEnumerable<DatasetChange>>> GetEnumerator()
        {
            return new ChangesEnumerator(_graphChanges);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private IList<DatasetChange> ChangesFor(EntityId graph)
        {
            if (!_graphChanges.ContainsKey(graph))
            {
                _graphChanges[graph] = new List<DatasetChange>(GraphChangesCapacity);
            }

            return _graphChanges[graph];
        }

        private class ChangesEnumerator : IEnumerator<KeyValuePair<EntityId, IEnumerable<DatasetChange>>>
        {
            private readonly IEnumerator<KeyValuePair<EntityId, IList<DatasetChange>>> _enumerator;

            public ChangesEnumerator(IEnumerable<KeyValuePair<EntityId, IList<DatasetChange>>> graphChanges)
            {
                _enumerator = graphChanges.GetEnumerator();
            }

            public KeyValuePair<EntityId, IEnumerable<DatasetChange>> Current
            {
                get
                {
                    return new KeyValuePair<EntityId, IEnumerable<DatasetChange>>(_enumerator.Current.Key, _enumerator.Current.Value);
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public void Dispose()
            {
                _enumerator.Dispose();
            }

            public bool MoveNext()
            {
                return _enumerator.MoveNext();
            }

            public void Reset()
            {
                _enumerator.Reset();
            }
        }
    }
}