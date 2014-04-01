using RomanticWeb.Mapping.Fluent;

namespace RomanticWeb.Collections.Mapping
{
    /// <summary>
    /// A base class for mapping dynamically created type for dictionary owner
    /// </summary>
    /// <typeparam name="T">Type of entity, which contains the dictionary</typeparam>
    /// <typeparam name="TPair">The type of the key/value pair.</typeparam>
    /// <typeparam name="TKey">The type of the dictionary key.</typeparam>
    /// <typeparam name="TValue">The type of the dictionary value.</typeparam>
    public abstract class DictionaryOwnerMap<T,TPair,TKey,TValue>:EntityMap<T>
        where T:IDictionaryOwner<TPair,TKey,TValue>
        where TPair:IDictionaryEntry<TKey,TValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryOwnerMap{T, TPair, TKey, TValue}"/> class.
        /// </summary>
        protected DictionaryOwnerMap()
        {
            SetupEntriesCollection(Collection(e => e.DictionaryEntries).Term);
        }

        /// <summary>
        /// Setups the entries collection property predicate.
        /// </summary>
        protected abstract void SetupEntriesCollection(ITermPart<ICollectionMap> term);
    }
}