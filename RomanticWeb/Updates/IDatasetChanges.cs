using System.Collections.Generic;
using RomanticWeb.Entities;

namespace RomanticWeb.Updates
{
    public interface IDatasetChanges : IEnumerable<KeyValuePair<EntityId, IEnumerable<DatasetChange>>>
    {
        bool HasChanges { get; }

        IEnumerable<DatasetChange> this[EntityId graphUri] { get; }
    }
}