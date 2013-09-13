namespace RomanticWeb
{
	public interface ITripleSourceFactory
	{
		ITripleSource CreateTriplesSourceForOntology();
		ITripleSource CreateTriplesSourceForEntity<T>(IMapping<T> mappingFor) where T:class,IEntity;
		ITripleSource CreateTripleSourceForProperty(EntityId entityId,IPropertyMapping property);
	}
}