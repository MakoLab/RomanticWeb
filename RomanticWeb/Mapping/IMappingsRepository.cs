using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Mapping
{
    /// <summary>
    /// Provides access to entity mappings
    /// </summary>
    public interface IMappingsRepository
	{
        /// <summary>
        /// Gets a mapping for an Entity type
        /// </summary>
        /// <typeparam name="TEntity">entity type</typeparam>
		IEntityMapping MappingFor<TEntity>();
	}
}