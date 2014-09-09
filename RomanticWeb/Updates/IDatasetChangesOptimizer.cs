using System.Collections.Generic;

namespace RomanticWeb.Updates
{
    public interface IDatasetChangesOptimizer
    {
        IEnumerable<DatasetChange> Optimize(IDatasetChanges changes);
    }
}