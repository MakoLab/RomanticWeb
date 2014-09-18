using RomanticWeb.Entities;

namespace RomanticWeb.Updates
{
    public class EntityDelete : DatasetChange
    {
        public EntityDelete(EntityId entity)
            : base(entity)
        {
        }
    }
}