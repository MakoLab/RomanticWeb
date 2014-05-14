using RomanticWeb.Entities;

namespace RomanticWeb
{
    internal interface IEntityCache
    {
        bool HasEntity(EntityId entityId);

        void Add(Entity entity);

        Entity Get(EntityId entityId);
    }
}