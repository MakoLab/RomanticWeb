using System;
using RomanticWeb.Mapping.Fluent;

namespace RomanticWeb.Collections.Mapping
{
    /// <summary>
    /// Base class for dynamically generated list mappings
    /// </summary>
    /// <typeparam name="TOwner">The type of the list owner.</typeparam>
    public abstract class ListOwnerMap<TOwner> : EntityMap<TOwner>
        where TOwner : IRdfListOwner
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListOwnerMap{TOwner}"/> class.
        /// </summary>
        protected ListOwnerMap()
        {
            Property(owner => owner.ListHead).Term.Is(ListPredicate);
        }

        /// <summary>
        /// Gets the list predicate uri
        /// </summary>
        protected abstract Uri ListPredicate { get; }
    }
}