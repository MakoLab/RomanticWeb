using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using RomanticWeb.Entities;

namespace RomanticWeb.Updates
{
    /// <summary>Represents changes made in the triple store.</summary>
    public sealed class DatasetChanges : IDatasetChangesTracker
    {
        private const int GraphChangesCapacity = 16;

        private readonly IDictionary<EntityId, IList<DatasetChange>> _graphChanges = new ConcurrentDictionary<EntityId, IList<DatasetChange>>();

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