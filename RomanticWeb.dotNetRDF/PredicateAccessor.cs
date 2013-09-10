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

        protected internal PredicateAccessor(ITripleStore tripleStore, Entity entityId, Ontology ontology, IEntityFactory entityFactory)
            : base(tripleStore, entityId, ontology, entityFactory)
        {
        }

        protected override IEnumerable<RdfNode> GetObjectNodes(ITripleStore triplesSource, Uri predicate)
        {
            IGraph sourceGraph = triplesSource.Graphs[null];

            INode entityNode = NodeFactory.CreateUriNode(EntityId.Uri);
            INode predicateNode = NodeFactory.CreateUriNode(predicate);

            return sourceGraph.GetTriplesWithSubjectPredicate(entityNode, predicateNode)
                              .Select(t=>WrapObjectNode(t.Object));
        }

        private static RdfNode WrapObjectNode(INode arg)
        {
            var literal = arg as ILiteralNode;
            if (literal != null)
            {
                return RdfNode.ForLiteral(literal.Value, literal.Language, literal.DataType);
            }
            var uriNode = arg as IUriNode;
            if (uriNode != null)
            {
                return RdfNode.ForUri(uriNode.Uri);
            }

            throw new NotImplementedException("Blank nodes aren't supported yet");
        }
    }
}