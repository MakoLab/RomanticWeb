namespace RomanticWeb
{
    public interface IMappingProvider
    {
        IMapping<TEntity> MappingFor<TEntity>();
    }
}