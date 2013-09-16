using System;
using System.Collections.Generic;
using System.Linq;

using NullGuard;

using RomanticWeb.Ontologies;

using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;

namespace RomanticWeb.DotNetRDF.TripleSources
{
    [NullGuard(ValidationFlags.OutValues)]
	public class UnionGraphSource:TripleSourceBase
	{
		public UnionGraphSource(ITripleStore tripleStore):base(CreateQueryStrategy(tripleStore))
		{
		}

		public override IEnumerable<RdfNode> GetObjectsForPredicate(EntityId entityId,Uri predicate)
		{
			return QueryStrategy.GetObjectsForPredicate(entityId,predicate);
		}

		public override bool TryGetListElements(RdfNode rdfList,out IEnumerable<RdfNode> listElements)
		{
			return QueryStrategy.TryGetListElements(rdfList,out listElements);
		}

		public override IEnumerable<Tuple<RdfNode,RdfNode,RdfNode>> GetNodesForQuery(string commandText)
		{
			SparqlQuery query=new SparqlQueryParser().ParseFromString(commandText);
			return QueryStrategy.GetNodesForQuery(query).Triples.Select(t => new Tuple<RdfNode,RdfNode,RdfNode>(t.Subject.WrapNode(),t.Predicate.WrapNode(),t.Object.WrapNode()));
		}

        private static IStoreQueryStrategy CreateQueryStrategy(ITripleStore tripleStore)
        {
            if (tripleStore is IInMemoryQueryableStore)
            {
                return new InMemoryApiStrategy((IInMemoryQueryableStore)tripleStore);
            }

            return new SparqlQueryStrategy((INativelyQueryableStore)tripleStore);
        }
	}
}