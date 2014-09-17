using System.Collections.Generic;
using RomanticWeb.Entities;

namespace RomanticWeb.Updates
{
    /// <summary>
    /// Declares a contract for reading changes made to the triple store
    /// </summary>
    public interface IDatasetChanges : IEnumerable<IEnumerable<DatasetChange>>
    {
        /// <summary>Gets a value indicating whether there are any changes.</summary>
        bool HasChanges { get; }

        /// <summary>
        /// Gets changes made to a single graph
        /// </summary>
        IEnumerable<DatasetChange> this[EntityId graphUri] { get; }
    }
}