using System;
using System.Collections.Generic;
using NullGuard;
using RomanticWeb.dotNetRDF.TripleSources;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Query.Datasets;

namespace RomanticWeb.dotNetRDF
{
	/// <summary>
	/// Entity factory implementation backed by a triple store
	/// </summary>
	public class TripleStoreTripleSourceFactory:TripleSourceFactoryBase
	{
		private readonly ITripleStore _tripleStore;

		/// <summary>
		/// Creates a new instance of <see cref="TripleStoreTripleSourceFactory"/>
		/// </summary>
		/// <param name="tripleStore">Triples will be read from this triple store</param>
		public TripleStoreTripleSourceFactory(ITripleStore tripleStore)
		{
			_tripleStore=tripleStore;
		}

		protected override ITripleSource CreateSourceForDefaultGraph()
		{
			return CreateSourceForNamedGraph(null);
		}

		protected override ITripleSource CreateSourceForNamedGraph([AllowNull] Uri namedGraph)
		{
			return new SingleGraphSource(_tripleStore[namedGraph]);
		}

		protected override ITripleSource CreateSourceForUnionGraph()
		{
			return new UnionGraphSource(_tripleStore);
		}
	}
}