namespace RomanticWeb.Entities
{
    /// <summary>
    /// Defines a contract for entities
    /// </summary>
    /// todo: add a generic IEntity os that the Id's type can be changed?
    public interface IEntity
    {
        /// <summary>Gets the entity's identifier</summary>
        EntityId Id { get; }

        /// <summary>Provides an accessor for dynamically gained properties of the entity.</summary>
        /// <param name="member">Name of the property to be accessed.</param>
        /// <returns>Returns a dynamic object representing a requested member or null.</returns>
        object this[string member] { get; }
    }
}