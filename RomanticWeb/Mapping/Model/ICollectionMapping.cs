namespace RomanticWeb.Mapping.Model
{
    /// <summary>
    /// Represents a collection property mapping
    /// </summary>
    public interface ICollectionMapping:IPropertyMapping
    {
        /// <summary>
        /// Gets the storage strategy for a mapped property
        /// </summary>
        StoreAs StoreAs { get; }
    }
}