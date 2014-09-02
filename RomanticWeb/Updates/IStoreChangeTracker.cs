using System.Collections.Generic;
using System.Linq;

namespace RomanticWeb.Updates
{
    public interface IStoreChangeTracker
    {
        bool HasChanges { get; }
    }

    internal class DefaultStoreChangeTracker : IStoreChangeTracker
    {
        private readonly IList<DatasetChange> _changes = new List<DatasetChange>(32);

        public bool HasChanges
        {
            get
            {
                return _changes.Any();
            }
        }
    }
}