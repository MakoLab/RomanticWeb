using System;
using System.Collections.Generic;
using RomanticWeb.Ontologies;

namespace RomanticWeb.DotNetRDF.TripleSources
{
	public abstract class TripleSourceBase:ITripleSource
	{
		private readonly IStoreQueryStrategy _queryStrategy;

		protected TripleSourceBase(IStoreQueryStrategy queryStrategy)
		{
			_queryStrategy=queryStrategy;
		}

		protected IStoreQueryStrategy QueryStrategy
		{
			get { return _queryStrategy; }
		}

		public abstract IEnumerable<Tuple<RdfNode,RdfNode,RdfNode>> GetNodesForQuery(string commandText);

		public abstract IEnumerable<RdfNode> GetObjectsForPredicate(EntityId entityId,Uri predicate);

		public abstract bool TryGetListElements(RdfNode rdfList,out IEnumerable<RdfNode> listElements);
	}
}