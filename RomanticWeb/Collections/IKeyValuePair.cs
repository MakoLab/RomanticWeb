using RomanticWeb.Entities;

namespace RomanticWeb.Collections
{
    public interface IKeyValuePair<TKey, TValue> : IEntity
    {
        TKey Key { get; set; }

        TValue Value { get; set; }
    }
}