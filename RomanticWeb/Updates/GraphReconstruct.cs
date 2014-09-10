using System.Collections.Generic;
using RomanticWeb.Entities;
using RomanticWeb.Model;

namespace RomanticWeb.Updates
{
    public class GraphReconstruct : DatasetChange
    {
        internal GraphReconstruct(EntityId changedEntity, EntityId graph, IEnumerable<EntityQuad> addedQuads)
            : base(changedEntity, graph)
        {
            AddedQuads = addedQuads;
        }

        public IEnumerable<EntityQuad> AddedQuads { get; private set; }
    }
}