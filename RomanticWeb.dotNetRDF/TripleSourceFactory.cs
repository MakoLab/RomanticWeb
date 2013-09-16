using System;

using NullGuard;
using RomanticWeb.DotNetRDF.TripleSources;
using VDS.RDF;

namespace RomanticWeb.DotNetRDF
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