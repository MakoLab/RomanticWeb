using System.Collections.Generic;
using RomanticWeb.Entities;

namespace RomanticWeb.Collections
{
    /// <summary>
    /// Base <see cref="IEntity" /> type for RDF dictionary
    /// </summary>
    /// <typeparam name="TEntry">The type of the dictionary entry entity.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public interface IDictionaryOwner<TEntry,TKey,TValue>:IEntity
        where TEntry:IDictionaryEntry<TKey,TValue>
    {
        /// <summary>
        /// Gets or sets the dictionary entries.
        /// </summary>
        ICollection<TEntry> DictionaryEntries { get; set; } 
    }
}