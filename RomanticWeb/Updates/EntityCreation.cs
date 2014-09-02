using RomanticWeb.Entities;
using RomanticWeb.Model;

namespace RomanticWeb.Updates
{
    internal class EntityCreation : DatasetChange
    {
        private readonly EntityQuad[] _quads;

        public EntityCreation(EntityId entityId, EntityQuad[] quads)
            : base(entityId)
        {
            _quads = quads;
        }
    }
}