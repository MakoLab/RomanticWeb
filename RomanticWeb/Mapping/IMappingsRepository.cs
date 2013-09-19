using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Mapping
{
    public interface IMappingsRepository
	{
		IMapping MappingFor<TEntity>();
	}
}