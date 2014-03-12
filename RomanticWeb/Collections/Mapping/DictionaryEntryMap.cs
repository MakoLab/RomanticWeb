using RomanticWeb.Mapping.Fluent;

namespace RomanticWeb.Collections.Mapping
{
    public abstract class DictionaryEntryMap<T, TKey, TValue> : EntityMap<T>
        where T : IKeyValuePair<TKey, TValue>
    {
        protected DictionaryEntryMap()
        {
            SetupKeyProperty(Property(e => e.Key).Term);
            SetupValueProperty(Property(e => e.Value).Term);
        }

        protected abstract void SetupKeyProperty(TermPart<PropertyMap> term);

        protected abstract void SetupValueProperty(TermPart<PropertyMap> term);
    }
}