namespace RomanticWeb
{
	public interface IEntity
	{
		/// <summary>Gets the entity's identifier</summary>
		EntityId Id { get; }

		TInterface AsEntity<TInterface>() where TInterface : class, IEntity;
	}
}