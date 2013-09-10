using System.Collections.Generic;
using RomanticWeb.Ontologies;

namespace RomanticWeb.dotNetRDF
{
    public class PredicateAccessor : RomanticWeb.PredicateAccessor
    {
        protected internal PredicateAccessor(ITriplesSource tripleStore, Entity entityId, Ontology ontology, IEntityFactory entityFactory)
            : base(tripleStore, entityId, ontology, entityFactory)
        {
        }

        protected override IEnumerable<RdfNode> GetObjectNodes(ITriplesSource triplesSource, Property predicate)
        {
            return triplesSource.GetObjectsForPredicate(EntityId, predicate);
        }
    }
}