using System;
using RomanticWeb.Mapping.Fluent;

namespace RomanticWeb.Collections.Mapping
{
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