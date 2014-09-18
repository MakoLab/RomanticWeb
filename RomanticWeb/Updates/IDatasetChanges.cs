using System.Collections.Generic;

namespace RomanticWeb.Updates
{
    /// <summary>
    /// Declares a contract for reading changes made to the triple store
    /// </summary>
    public interface IDatasetChanges : IEnumerable<DatasetChange>
    {
        /// <summary>Gets a value indicating whether there are any changes.</summary>
        bool HasChanges { get; }
    }
}