using System.Collections.Generic;
using System.Linq;
using RomanticWeb.Entities;
using RomanticWeb.Model;

namespace RomanticWeb.Updates
{
    /// <summary>
    /// Represents a change, which updates a named graph
    /// </summary>
    public class GraphUpdate : DatasetChange
    {
        private readonly ISet<EntityQuad> _removedQuads;
        private readonly ISet<EntityQuad> _addedQuads;

        internal GraphUpdate(EntityId entityId, EntityId graph, EntityQuad[] removedQuads, EntityQuad[] addedQuads)
            : base(entityId, graph)
        {
            _removedQuads = new HashSet<EntityQuad>(removedQuads.Except(addedQuads));
            _addedQuads = new HashSet<EntityQuad>(addedQuads.Except(removedQuads));
        }

        /// <summary>
        /// Gets the removed quads.
        /// </summary>
        public IEnumerable<EntityQuad> RemovedQuads
        {
            get
            {
                return _removedQuads;
            }
        }

        /// <summary>
        /// Gets the added quads.
        /// </summary>
        public IEnumerable<EntityQuad> AddedQuads
        {
            get
            {
                return _addedQuads;
            }
        }
    }
}