using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RomanticWeb.Mapping.Visitors
{
    internal class MappingProviderVisitorChain : IMappingProviderVisitorChain
    {
        private readonly IList<Type> _visitors = new List<Type>();

        public IEnumerable<Type> Visitors
        {
            get
            {
                return new ReadOnlyCollection<Type>(_visitors);
            }
        }

        public void AddFirst<T>() where T : IMappingProviderVisitor
        {
            _visitors.Insert(0, typeof(T));
        }

        public void AddLast<T>() where T : IMappingProviderVisitor
        {
            _visitors.Add(typeof(T));
        }

        public void AddAfter<TExisting, TNew>() where TExisting : IMappingProviderVisitor where TNew : IMappingProviderVisitor
        {
            var indexOfExisting = _visitors.IndexOf(typeof(TExisting));
            if (indexOfExisting == -1)
            {
                AddLast<TNew>();
            }

            _visitors.Insert(indexOfExisting + 1, typeof(TNew));
        }
    }
}