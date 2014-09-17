using System.Collections.Generic;
using System.Linq;
using Anotar.NLog;

namespace RomanticWeb.Updates
{
    internal class DatasetChangesOptimizer : IDatasetChangesOptimizer
    {
        public IEnumerable<DatasetChange> Optimize(IDatasetChanges changes)
        {
            foreach (var change in changes.SelectMany(OptimizeChangesForGraph))
            {
                LogTo.Info("Selecting change '{0}' for committing to store", change);
                yield return change;
            }
        }

        private IEnumerable<DatasetChange> OptimizeChangesForGraph(IEnumerable<DatasetChange> changesForGraph)
        {
            var result = new Stack<DatasetChange>();

            foreach (var change in changesForGraph)
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

            return new GraphUpdate(compacted.Entity, compacted.Graph, removed.ToArray(), added.ToArray());
        }
    }
}