using System;

namespace RomanticWeb
{
	public abstract class TripleSourceFactoryBase:ITripleSourceFactory
	{
		public ITripleSource CreateTriplesSourceForOntology()
		{
			return CreateSourceForUnionGraph();
		}

		public ITripleSource CreateTriplesSourceForEntity<TEntity>(IMapping<TEntity> mappingFor) where TEntity:class,IEntity
		{
			return CreateSourceForUnionGraph();
		}

		public ITripleSource CreateTripleSourceForProperty(EntityId entityId,IPropertyMapping property)
		{
			if (property.GraphSelector!=null)
			{
				var namedGraphUri=property.GraphSelector.SelectGraph(entityId);
				return CreateSourceForNamedGraph(namedGraphUri);
			}
			if (property.UsesUnionGraph)
			{
				return CreateSourceForUnionGraph();
			}

			return CreateSourceForDefaultGraph();
		}

		protected abstract ITripleSource CreateSourceForDefaultGraph();
		protected abstract ITripleSource CreateSourceForNamedGraph(Uri namedGraph);
		protected abstract ITripleSource CreateSourceForUnionGraph();
	}
}