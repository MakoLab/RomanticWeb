using System.Collections.Generic;
using System.Linq;
using RomanticWeb.Entities;
using RomanticWeb.Model;

namespace RomanticWeb.Updates
{
    /// <summary>
    /// Represents a change with recreates a named graph
    /// </summary>
    public class GraphReconstruct : DatasetChange
    {
        internal GraphReconstruct(EntityId entity, EntityId graph, IEnumerable<EntityQuad> addedQuads)
            : base(entity, graph)
        {
            AddedQuads = addedQuads;
        }

        /// <summary>
        /// Gets recreated graph's content
        /// </summary>
        public IEnumerable<EntityQuad> AddedQuads { get; private set; }

        public override string ToString()
        {
            return string.Format("Recreating graph {0} with {1} triples", Graph, AddedQuads.Count());
        }
    }
}