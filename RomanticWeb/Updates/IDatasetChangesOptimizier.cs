using System.Collections.Generic;

namespace RomanticWeb.Updates
{
    public interface IDatasetChangesOptimizier
    {
        IEnumerable<DatasetChange> Optimize(IDatasetChanges changes);
    }

    internal class DatasetChangesOptimizier : IDatasetChangesOptimizier
    {
        public IEnumerable<DatasetChange> Optimize(IDatasetChanges changes)
        {
            throw new System.NotImplementedException();
        }
    }
}