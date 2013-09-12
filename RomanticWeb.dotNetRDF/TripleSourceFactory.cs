using System;
using RomanticWeb.dotNetRDF.TripleSources;
using VDS.RDF;

namespace RomanticWeb.dotNetRDF
{
    /// <summary>
    /// Entity factory implementation backed by a triple store
    /// </summary>
    public class TripleStoreTripleSourceFactory : TripleSourceFactoryBase
    {
        private readonly ITripleStore _tripeStore;

        /// <summary>
        /// Creates a new instance of <see cref="TripleStoreTripleSourceFactory"/>
        /// </summary>
        /// <param name="tripeStore">Triples will be read from this triple store</param>
        public TripleStoreTripleSourceFactory(ITripleStore tripeStore)
        {
            _tripeStore = tripeStore;
        }

        public override ITripleSource CreateSourceForGraph(Uri namedGraph)
        {
            return new SingleGraphSource(_tripeStore[namedGraph]);
        }

        public override ITripleSource CreateSourceForUnionGraph()
        {
            return new UnionGraphSource(_tripeStore);
        }
    }
}
