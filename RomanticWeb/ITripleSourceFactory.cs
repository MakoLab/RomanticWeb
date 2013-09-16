namespace RomanticWeb
{
    using RomanticWeb.Mapping.Model;

    public interface ITripleSourceFactory
	{
		ITripleSource CreateTriplesSourceForOntology();

		ITripleSource CreateTriplesSourceForEntity<TEntity>(IMapping mappingFor) where TEntity : class;

		ITripleSource CreateTripleSourceForProperty(EntityId entityId, IPropertyMapping property);
	}
}