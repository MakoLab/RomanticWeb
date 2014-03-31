using System.Collections.Generic;
using RomanticWeb.Entities;

namespace RomanticWeb.Collections
{
    internal interface IRdfDictionary
    {
        IEnumerable<IEntity> DictionaryEntries { get; } 
    }
}