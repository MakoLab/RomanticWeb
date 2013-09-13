namespace RomanticWeb
{
	public interface IMappingProvider
	{
		IMapping<T> MappingFor<T>() where T:IEntity;
	}
}