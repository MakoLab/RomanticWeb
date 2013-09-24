using System;
using System.Collections.Generic;
using RomanticWeb.Entities;
using RomanticWeb.Ontologies;

namespace RomanticWeb
{
    public interface IEntityStore
    {
        IEnumerable<RdfNode> GetObjectsForPredicate(EntityId entity,Uri predicate);

        void AssertTriple(Tuple<RdfNode, RdfNode, RdfNode, RdfNode> entityTriple);

        bool TripleIsCollectionRoot(IEntity potentialList);
    }
}