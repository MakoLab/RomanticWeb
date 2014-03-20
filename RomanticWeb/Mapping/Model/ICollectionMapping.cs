namespace RomanticWeb.Mapping.Model
{
    public interface ICollectionMapping:IPropertyMapping
    {
        /// <summary>
        /// Gets the storage strategy for a mapped property
        /// </summary>
        StoreAs StoreAs { get; }
    }
}