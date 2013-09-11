using System.Collections.Generic;
using RomanticWeb.Ontologies;

namespace RomanticWeb.dotNetRDF.TripleSources
{
    public interface IStoreQueryStrategy
    {
        IEnumerable<RdfNode> GetObjectsForPredicate(EntityId entityId, Property predicate);
        bool TryGetListElements(RdfNode rdfList, out IEnumerable<RdfNode> listElements);
    }
}