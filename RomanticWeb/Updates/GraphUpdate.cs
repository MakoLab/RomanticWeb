using System.Collections.Generic;
using RomanticWeb.Entities;
using RomanticWeb.Model;

namespace RomanticWeb.Updates
{
    internal class GraphUpdate : DatasetChange
    {
        private readonly EntityQuad[] _removedQuads;
        private readonly EntityQuad[] _addedQuads;

        public GraphUpdate(EntityId entityId, EntityId graph, EntityQuad[] removedQuads, EntityQuad[] addedQuads)
            : base(entityId, graph)
        {
            _removedQuads = removedQuads;
            _addedQuads = addedQuads;
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