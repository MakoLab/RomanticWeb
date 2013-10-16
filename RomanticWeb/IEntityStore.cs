using System;
using System.Collections.Generic;
using RomanticWeb.Entities;
using RomanticWeb.Model;

namespace RomanticWeb
{
    public interface IEntityStore
    {
        IEnumerable<EntityTriple> Quads { get; }

        IEnumerable<Node> GetObjectsForPredicate(EntityId entityId,Uri predicate);

        void AssertTriple(EntityTriple entityTriple);
    }
}