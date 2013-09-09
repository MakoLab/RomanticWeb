namespace RomanticWeb
{
    /// <summary>
    /// Defines methods for factories, which produce <see cref="Entity"/> instances
    /// </summary>
    public interface IEntityFactory
    {
        /// <summary>
        /// Creates a new Entity
        /// </summary>
        /// <param name="entityId">Entity identifier</param>
        Entity Create(EntityId entityId);
    }
}