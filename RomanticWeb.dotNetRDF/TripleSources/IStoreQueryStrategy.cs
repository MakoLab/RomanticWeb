using System;
using System.Collections.Generic;
using RomanticWeb.Entities;
using RomanticWeb.Ontologies;
using VDS.RDF;
using VDS.RDF.Query;

namespace RomanticWeb.DotNetRDF.TripleSources
{
	public interface IStoreQueryStrategy
	{
		ITripleStore GetNodesForQuery(SparqlQuery query);

		IEnumerable<RdfNode> GetObjectsForPredicate(EntityId entityId,Uri predicate);

		bool TryGetListElements(RdfNode rdfList,out IEnumerable<RdfNode> listElements);
	}
}