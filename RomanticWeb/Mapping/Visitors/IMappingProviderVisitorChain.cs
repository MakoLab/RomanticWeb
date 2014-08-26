using System;
using System.Collections.Generic;

namespace RomanticWeb.Mapping.Visitors
{
    public interface IMappingProviderVisitorChain
    {
        IEnumerable<Type> Visitors { get; }

        void AddFirst<T>() where T : IMappingProviderVisitor;
        
        void AddLast<T>() where T : IMappingProviderVisitor;

        void AddAfter<TExisting, TNew>() 
            where TExisting : IMappingProviderVisitor 
            where TNew : IMappingProviderVisitor;
    }
}