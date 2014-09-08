namespace RomanticWeb.Updates
{
    public interface IDatasetChangesTracker : IDatasetChanges
    {
        void Add(DatasetChange datasetChange);
    }
}