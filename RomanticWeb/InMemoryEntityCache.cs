using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using RomanticWeb.Entities;

namespace RomanticWeb
{
    internal class InMemoryEntityCache : IEntityCache
    {
        private readonly IDictionary<EntityId, Entity> _cache = new ConcurrentDictionary<EntityId, Entity>();

        public bool HasEntity(EntityId entityId)
        {
            return _cache.ContainsKey(entityId);
        }

        public void Add(Entity entity)
        {
            _cache[entity.Id] = entity;
        }

        public Entity Get(EntityId entityId)
        {
            return _cache[entityId];
        }
    }
}