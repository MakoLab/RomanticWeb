using System.Collections.Generic;
using System.Linq;
using RomanticWeb.Entities;
using RomanticWeb.Model;

namespace RomanticWeb.Updates
{
    internal class GraphUpdate : DatasetChange
    {
        private readonly ISet<EntityQuad> _removedQuads;
        private readonly ISet<EntityQuad> _addedQuads;

        public GraphUpdate(EntityId entityId, EntityId graph, EntityQuad[] removedQuads, EntityQuad[] addedQuads)
            : base(entityId, graph)
        {
            _removedQuads = new HashSet<EntityQuad>(removedQuads.Except(addedQuads));
            _addedQuads = new HashSet<EntityQuad>(addedQuads.Except(removedQuads));
        }

        public IEnumerable<EntityQuad> RemovedQuads
        {
            get
            {
                return _removedQuads;
            }
        }

        public IEnumerable<EntityQuad> AddedQuads
        {
            get
            {
                return _addedQuads;
            }
        }
    }
}