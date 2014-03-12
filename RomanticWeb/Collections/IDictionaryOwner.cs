using System.Collections.Generic;
using RomanticWeb.Entities;

namespace RomanticWeb.Collections
{
    public interface IDictionaryOwner<TPair,TKey,TValue>:IEntity
        where TPair:IKeyValuePair<TKey,TValue>
    {
        ICollection<TPair> DictionaryEntries { get; set; } 
    }
}