using System.Collections.Generic;

namespace RomanticWeb.Updates
{
    /// <summary>
    /// Optimizes the specified changes to minimize redundant operations on triple store
    /// </summary>
    public interface IDatasetChangesOptimizer
    {
        /// <summary>
        /// Optimizes the changes
        /// </summary>
        IEnumerable<DatasetChange> Optimize(IDatasetChanges changes);
    }
}