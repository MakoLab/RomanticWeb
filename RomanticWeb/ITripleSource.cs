using System;
using System.Collections.Generic;
using RomanticWeb.Ontologies;

namespace RomanticWeb
{
    public interface ITripleSource
    {
        IEnumerable<RdfNode> GetObjectsForPredicate(EntityId entityId, Uri predicate);
        bool TryGetListElements(RdfNode rdfList, out IEnumerable<RdfNode> listElements);
    }
}