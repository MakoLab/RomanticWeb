namespace RomanticWeb.Mapping
{
    using RomanticWeb.Mapping.Model;

    public interface IMappingsRepository
	{
		IMapping MappingFor<TEntity>();
	}
}