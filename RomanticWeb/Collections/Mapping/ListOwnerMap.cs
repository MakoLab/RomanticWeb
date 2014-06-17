using System;
using RomanticWeb.Mapping.Fluent;

namespace RomanticWeb.Collections.Mapping
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TOwner">The type of the list owner.</typeparam>
    public abstract class ListOwnerMap<TOwner> : EntityMap<TOwner>
        where TOwner : IRdfListOwner
    {
        protected ListOwnerMap()
        {
            Property(owner => owner.ListHead).Term.Is(ListPredicate);
        }

        protected abstract Uri ListPredicate { get; }
    }
}