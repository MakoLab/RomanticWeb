using System.Collections.Generic;
using System.Linq;
using RomanticWeb.Entities;

namespace RomanticWeb.Updates
{
    internal class DatasetChangesOptimizer : IDatasetChangesOptimizer
    {
        public IEnumerable<DatasetChange> Optimize(IDatasetChanges changes)
        {
            return changes.SelectMany(OptimizeChangesForGraph);
        }

        private IEnumerable<DatasetChange> OptimizeChangesForGraph(KeyValuePair<EntityId, IEnumerable<DatasetChange>> changesForGraph)
        {
            var result = new Stack<DatasetChange>();

            foreach (var change in changesForGraph.Value)
            {
                if (change is GraphDelete)
                {
                    result.Clear();
                    result.Push(change);
                }
                else
                {
                    if (result.Count > 0 && result.Peek() is GraphUpdate)
                    {
                        result.Push(MergeUpdates((GraphUpdate)result.Pop(), (GraphUpdate)change));
                    }
                    else
                    {
                        result.Push(change);
                    }
                }
            }

            return result;
        }

        private DatasetChange MergeUpdates(GraphUpdate compacted, GraphUpdate nextChange)
        {
            var removed = compacted.RemovedQuads.ToList();
            var added = compacted.AddedQuads.ToList();

            return new GraphUpdate(compacted.ChangedEntity, compacted.Graph, removed.ToArray(), added.ToArray());
        }
    }
}