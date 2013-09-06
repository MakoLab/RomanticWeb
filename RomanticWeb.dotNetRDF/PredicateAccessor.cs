using System;
using System.Collections.Generic;
using System.Linq;
using VDS.RDF;

namespace RomanticWeb.dotNetRDF
{
    public class PredicateAccessor : PredicateAccessor<ITripleStore>
    {
        static readonly NodeFactory NodeFactory = new NodeFactory();

        public PredicateAccessor(ITripleStore tripleStore, Entity entityId, Ontology ontology)
            : base(tripleStore, entityId, ontology)
        {
        }

        protected override IEnumerable<string> GetObjects(ITripleStore triplesSource, Uri baseUri, Property predicate)
        {
            IGraph sourceGraph = triplesSource.Graphs[null];

            INode entityNode = NodeFactory.CreateUriNode(EntityId.Uri);
            INode predicateNode = NodeFactory.CreateUriNode(new Uri(baseUri, predicate.PredicateUri));

            return sourceGraph.GetTriplesWithSubjectPredicate(entityNode, predicateNode)
                              .Select(s => s.Object.ToString());
        }
    }
}