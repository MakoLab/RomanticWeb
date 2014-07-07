using RomanticWeb.Entities;

namespace RomanticWeb.Collections
{
    /// <summary>
    /// Entity type for RDF dictionary entries (key/value pairs)
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public interface IDictionaryEntry<TKey, TValue> : IEntity
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        TKey Key { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        TValue Value { get; set; }
    }
}