using System;
using System.Collections.Generic;
using System.Linq;
using NullGuard;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Mapping
{
    /// <summary>
    /// Implementation of <see cref="IRdfTypeCache"/>, 
    /// which built by visiting <see cref="IEntityMapping"/>s
    /// </summary>
    public class RdfTypeCache : IRdfTypeCache
    {
        private readonly IDictionary<string, IEnumerable<Type>> _cache;
        private readonly IDictionary<Type, IList<IClassMapping>> _classMappings;
        private readonly IDictionary<Type, ISet<Type>> _directlyDerivingTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="RdfTypeCache"/> class.
        /// </summary>
        public RdfTypeCache()
        {
            _classMappings = new Dictionary<Type, IList<IClassMapping>>();
            _directlyDerivingTypes = new Dictionary<Type, ISet<Type>>();
            _cache = new Dictionary<string, IEnumerable<Type>>();
        }

        /// <inheridoc/>
        [return: AllowNull]
        public IEnumerable<Type> GetMostDerivedMappedTypes(IEnumerable<Uri> entityTypes, Type requestedType)
        {
            ISet<Type> selectedTypes = new HashSet<Type> { requestedType };
            if (requestedType == typeof(ITypedEntity))
            {
                return selectedTypes;
            }

            IEnumerable<Type> cached;
            var classList = entityTypes as Uri[] ?? entityTypes.ToArray();
            string cacheKey = requestedType + ";" + String.Join(";", classList.Select(item => item.ToString()));
            if (!_cache.TryGetValue(cacheKey, out cached))
            {
                if ((classList.Any()) && (_directlyDerivingTypes.ContainsKey(requestedType)))
                {
                    var childTypesToCheck = new Queue<Type>(_directlyDerivingTypes[requestedType]);
                    while (childTypesToCheck.Any())
                    {
                        Type potentialMatch = childTypesToCheck.Dequeue();

                        if (_directlyDerivingTypes.ContainsKey(potentialMatch))
                        {
                            foreach (var child in _directlyDerivingTypes[potentialMatch])
                            {
                                childTypesToCheck.Enqueue(child);
                            }
                        }

                        if (_classMappings.ContainsKey(potentialMatch))
                        {
                            foreach (var mapping in _classMappings[potentialMatch])
                            {
                                if (mapping.IsMatch(classList))
                                {
                                    selectedTypes.Add(potentialMatch);
                                }
                            }
                        }
                    }
                }

                _cache[cacheKey] = cached = selectedTypes.GetMostDerivedTypes();
            }

            return cached;
        }

        /// <inheridoc/>
        public void Add(Type entityType, IList<IClassMapping> classMappings)
        {
            AddAsChildOfParentTypes(entityType);
            _classMappings[entityType] = classMappings;
        }

        private void AddAsChildOfParentTypes(Type entityType)
        {
            foreach (var parentType in entityType.GetImmediateParents(false))
            {
                if (!_directlyDerivingTypes.ContainsKey(parentType))
                {
                    _directlyDerivingTypes.Add(parentType, new HashSet<Type>());
                }

                _directlyDerivingTypes[parentType].Add(entityType);
            }
        }
    }
}