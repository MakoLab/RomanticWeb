using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RomanticWeb.Mapping.Visitors
{
    internal class MappingProviderVisitorChain
    {
        private readonly IList<Type> _visitors = new List<Type>();

        public IEnumerable<Type> Visitors
        {
            get
            {
                return new ReadOnlyCollection<Type>(_visitors);
            }
        }

        public void Add<T>() where T : IMappingProviderVisitor
        {
            _visitors.Add(typeof(T));
        }
    }
}