using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using RomanticWeb.Entities;

namespace RomanticWeb.Updates
{
    /// <summary>Represents ordered changes made in the triple store.</summary>
    public sealed class DatasetChanges : IDatasetChangesTracker
    {
        private const int GraphChangesCapacity = 16;
        private readonly IList<DatasetChange> _datesetWideChanges = new List<DatasetChange>();
        private readonly IDictionary<EntityId, IList<DatasetChange>> _graphChanges = new ConcurrentDictionary<EntityId, IList<DatasetChange>>();

        /// <inheritdoc/>
        public bool HasChanges
        {
            get
            {
                return _graphChanges.Any() && _graphChanges.All(changes => changes.Value.Any());
            }
        }

        /// <inheritdoc/>
        public IEnumerable<DatasetChange> this[EntityId graphUri]
        {
            get
            {
                return _graphChanges[graphUri];
            }
        }

        /// <inheritdoc/>
        public void Add(DatasetChange datasetChange)
        {
            ChangesFor(datasetChange.Graph).Add(datasetChange);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            _graphChanges.Clear();
        }

        /// <summary>
        /// Gets the enumerator of changes grouped by named graphs
        /// </summary>
        public IEnumerator<IEnumerable<DatasetChange>> GetEnumerator()
        {
            foreach (var changes in _graphChanges)
            {
                yield return changes.Value;
            }

            yield return _datesetWideChanges;
        }

        /// <summary>
        /// Gets the enumerator of changes grouped by named graphs
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private IList<DatasetChange> ChangesFor(EntityId graph)
        {
            if (graph == null)
            {
                return _datesetWideChanges;
            }

            if (!_graphChanges.ContainsKey(graph))
            {
                _graphChanges[graph] = new List<DatasetChange>(GraphChangesCapacity);
            }

            return _graphChanges[graph];
        }
    }
}