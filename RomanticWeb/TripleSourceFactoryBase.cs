using System;

namespace RomanticWeb
{
    public abstract class TripleSourceFactoryBase : ITripleSourceFactory
    {
        public ITripleSource CreateTriplesSourceForOntology()
        {
            return CreateSourceForUnionGraph();
        }

        public ITripleSource CreateTriplesSourceForEntity<TEntity>(IMapping<TEntity> mappingFor) where TEntity : class
        {
            return CreateSourceForUnionGraph();
        }

        public abstract ITripleSource CreateSourceForGraph(Uri namedGraph);
        public abstract ITripleSource CreateSourceForUnionGraph();
    }
}