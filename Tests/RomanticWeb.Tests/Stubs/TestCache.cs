using System;
using System.Collections.Generic;
using RomanticWeb.Entities;
using RomanticWeb.Mapping;

namespace RomanticWeb.Tests.Stubs
{
    public class TestCache:IRdfTypeCache
    {
        private readonly IDictionary<Type,Type> _setups;

        public TestCache()
        {
            _setups=new Dictionary<Type,Type>();
        }

        public IEnumerable<Type> GetMostDerivedMappedTypes(IEntity entity,Type requestedType)
        {
            if (_setups.ContainsKey(requestedType))
            {
                yield return _setups[requestedType];
            }
            else
            {
                yield return requestedType;
            }
        }

        public void Setup<TRequested,TReturned>()
        {
            _setups[typeof(TRequested)]=typeof(TReturned);
        }
    }
}