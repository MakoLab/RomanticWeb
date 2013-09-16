namespace RomanticWeb
{
    public interface IMappingsRepository
    {
        IMapping MappingFor<TEntity>();
    }
}