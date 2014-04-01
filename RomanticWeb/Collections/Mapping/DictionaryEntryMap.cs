using RomanticWeb.Mapping.Fluent;

namespace RomanticWeb.Collections.Mapping
{
    /// <summary>
    /// A base class for mapping dynamically created type for dictionary entries
    /// </summary>
    /// <typeparam name="T">Type of entity, which contains the dictionary</typeparam>
    /// <typeparam name="TKey">The type of the dictionary key.</typeparam>
    /// <typeparam name="TValue">The type of the dictionary value.</typeparam>
    public abstract class DictionaryEntryMap<T,TKey,TValue>:EntityMap<T>
        where T:IDictionaryEntry<TKey,TValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryEntryMap{T, TKey, TValue}"/> class.
        /// </summary>
        protected DictionaryEntryMap()
        {
            SetupKeyProperty(Property(e => e.Key).Term);
            SetupValueProperty(Property(e => e.Value).Term);
        }

        /// <summary>
        /// Setups the key property mapping.
        /// </summary>
        protected abstract void SetupKeyProperty(ITermPart<IPropertyMap> term);

        /// <summary>
        /// Setups the value property mapping.
        /// </summary>
        protected abstract void SetupValueProperty(ITermPart<IPropertyMap> term);
    }
}