using System;
using System.Collections.Generic;
using RomanticWeb.Ontologies;
using VDS.RDF;
using VDS.RDF.Query;

namespace RomanticWeb.dotNetRDF.TripleSources
{
	public interface IStoreQueryStrategy
	{
		ITripleStore GetNodesForQuery(SparqlQuery query);
		IEnumerable<RdfNode> GetObjectsForPredicate(EntityId entityId,Uri predicate);
		bool TryGetListElements(RdfNode rdfList,out IEnumerable<RdfNode> listElements);
	}
}