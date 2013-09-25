﻿using System;
using System.Collections.Generic;
using RomanticWeb.Entities;
using RomanticWeb.Ontologies;

namespace RomanticWeb
{
    public interface IEntityStore
    {
        IEnumerable<Node> GetObjectsForPredicate(EntityId entityId,Uri predicate);

        bool EntityIsCollectionRoot(IEntity potentialList);

        void AssertTriple(EntityId entityId,Node graph,Triple triple);
    }
}