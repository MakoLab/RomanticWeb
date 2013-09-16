namespace RomanticWeb
{
	public interface ITripleSourceFactory
	{
		ITripleSource CreateTriplesSourceForOntology();

		ITripleSource CreateTriplesSourceForEntity<TEntity>(IMapping mappingFor) where TEntity : class;

		ITripleSource CreateTripleSourceForProperty(EntityId entityId, IPropertyMapping property);
	}
}