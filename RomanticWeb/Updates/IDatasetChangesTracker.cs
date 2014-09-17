namespace RomanticWeb.Updates
{
    /// <summary>
    /// Declares a contract for tracking changes made to the triple store
    /// </summary>
    public interface IDatasetChangesTracker : IDatasetChanges
    {
        /// <summary>
        /// Adds a dataset change
        /// </summary>
        void Add(DatasetChange datasetChange);

        /// <summary>
        /// Removes all pendeingchanges
        /// </summary>
        void Clear();
    }
}