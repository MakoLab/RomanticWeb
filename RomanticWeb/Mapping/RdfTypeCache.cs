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
    public class RdfTypeCache : IRdfTypeCache, Visitors.IMappingModelVisitor
    {
        private readonly IDictionary<Type, IList<IClassMapping>> _classMappings;
        private readonly IDictionary<Type, ISet<Type>> _directlyDerivingTypes;
        private IList<IClassMapping> _currentClasses;

        /// <summary>
        /// Initializes a new instance of the <see cref="RdfTypeCache"/> class.
        /// </summary>
        public RdfTypeCache()
        {
            _classMappings = new Dictionary<Type, IList<IClassMapping>>();
            _directlyDerivingTypes = new Dictionary<Type, ISet<Type>>();
        }

        /// <inheridoc/>
        [return: AllowNull]
        public IEnumerable<Type> GetMostDerivedMappedTypes(IEntity entity, Type requestedType)
        {
            if (requestedType == typeof(ITypedEntity))
            {
                return new[] { typeof(ITypedEntity) };
            }

            ISet<Type> selectedTypes = new HashSet<Type> { requestedType };
            var types = entity.AsEntity<ITypedEntity>().Types.Select(t => t.Uri).ToList();

            if (types.Any() && _directlyDerivingTypes.ContainsKey(requestedType))
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
                            if (mapping.IsMatch(types))
                            {
                                selectedTypes.Add(potentialMatch);
                            }
                        }
                    }
                }
            }

            return selectedTypes.GetMostDerivedTypes();
        }

        /// <summary>
        /// Sets the currently processed enitty type
        /// and updates inheritance cache
        /// </summary>
        public void Visit(IEntityMapping entityMapping)
        {
            if (!_classMappings.ContainsKey(entityMapping.EntityType))
            {
                _classMappings.Add(entityMapping.EntityType, new List<IClassMapping>());
            }

            AddAsChildOfParentTypes(entityMapping.EntityType);

            _currentClasses = _classMappings[entityMapping.EntityType];
        }

        /// <summary>
        /// Does nothing
        /// </summary>
        public void Visit(ICollectionMapping collectionMapping)
        {
        }

        /// <summary>
        /// Does nothing
        /// </summary>
        public void Visit(IDictionaryMapping dictionaryMapping)
        {
        }

        /// <summary>
        /// Does nothing
        /// </summary>
        public void Visit(IPropertyMapping propertyMapping)
        {
        }

        /// <summary>
        /// Adds class URI to the current entity's list
        /// </summary>
        public void Visit(IClassMapping classMapping)
        {
            if (!classMapping.IsInherited)
            {
                _currentClasses.Add(classMapping);
            }
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