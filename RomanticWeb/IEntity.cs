namespace RomanticWeb
{
    internal interface IEntity
    {
        /// <summary>
        /// Gets the entity's identifier
        /// </summary>
        EntityId Id { get; }
    }
}