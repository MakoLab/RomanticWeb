using System;
using System.Collections.Generic;
using System.Linq;
using RomanticWeb.Ontologies;
using VDS.RDF;

namespace RomanticWeb.dotNetRDF
{
    public class PredicateAccessor : PredicateAccessor<ITripleStore>
    {
        static readonly NodeFactory NodeFactory = new NodeFactory();

        internal PredicateAccessor(ITripleStore tripleStore, Entity entityId, Ontology ontology, IEntityFactory entityFactory)
            : base(tripleStore, entityId, ontology, entityFactory)
        {
        }

        protected override IEnumerable<RdfNode> GetObjectNodes(ITripleStore triplesSource, Uri baseUri, Property predicate)
        {
            IGraph sourceGraph = triplesSource.Graphs[null];

            INode entityNode = NodeFactory.CreateUriNode(EntityId.Uri);
            INode predicateNode = NodeFactory.CreateUriNode(new Uri(baseUri + predicate.PredicateUri));

            return sourceGraph.GetTriplesWithSubjectPredicate(entityNode, predicateNode)
                              .Select(t=>WrapObjectNode(t.Object));
        }

        private static RdfNode WrapObjectNode(INode arg)
        {
            ILiteralNode literal = arg as ILiteralNode;
            if (literal != null)
            {
                return RdfNode.ForLiteral(literal.Value, literal.Language, literal.DataType);
            }
            IUriNode uriNode = arg as IUriNode;
            if (uriNode != null)
            {
                return RdfNode.ForUri(uriNode.Uri);
            }

            throw new NotImplementedException("Blank nodes aren't supported yet");
        }
    }
}