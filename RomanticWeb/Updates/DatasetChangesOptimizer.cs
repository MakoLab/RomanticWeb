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
                else if (change is GraphReconstruct)
                {
                    result = new Stack<DatasetChange>(result.Where(c => c is GraphDelete));
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

            return result.Reverse();
        }

        private DatasetChange MergeUpdates(GraphUpdate compacted, GraphUpdate nextChange)
        {
            var removed = compacted.RemovedQuads.Union(nextChange.RemovedQuads);
            var added = compacted.AddedQuads.Union(nextChange.AddedQuads);

            return new GraphUpdate(compacted.ChangedEntity, compacted.Graph, removed.ToArray(), added.ToArray());
        }
    }
}