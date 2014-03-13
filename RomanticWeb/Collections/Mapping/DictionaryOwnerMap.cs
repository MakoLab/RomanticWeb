using RomanticWeb.Mapping.Fluent;

namespace RomanticWeb.Collections.Mapping
{
    public abstract class DictionaryOwnerMap<T, TPair, TKey, TValue> : EntityMap<T>
        where T : IDictionaryOwner<TPair, TKey, TValue>
        where TPair : IKeyValuePair<TKey, TValue>
    {
        protected DictionaryOwnerMap()
        {
            SetupEntriesCollection(Collection(e => e.DictionaryEntries).Term);
        }

        protected abstract void SetupEntriesCollection(ITermPart<CollectionMap> term);
    }
}