namespace RomanticWeb
{
    public interface IEntityFactory
    {
        /// <summary>
        /// Creates a new Entity
        /// </summary>
        /// <param name="entityId">Entity identifier</param>
        Entity Create(EntityId entityId);
    }
}