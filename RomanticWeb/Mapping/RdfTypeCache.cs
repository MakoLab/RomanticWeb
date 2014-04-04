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
    public class RdfTypeCache:IRdfTypeCache,Visitors.IMappingModelVisitor
    {
        private readonly IDictionary<Type,ISet<Uri>> _classUris;
        private readonly IDictionary<Type,ISet<Type>> _directlyDerivingTypes;
        private ISet<Uri> _currentClasses;

        /// <summary>
        /// Initializes a new instance of the <see cref="RdfTypeCache"/> class.
        /// </summary>
        public RdfTypeCache()
        {
            _classUris = new Dictionary<Type, ISet<Uri>>();
            _directlyDerivingTypes = new Dictionary<Type, ISet<Type>>();
        }

        /// <inheridoc/>
        [return: AllowNull]
        public Type GetMostDerivedMappedType(IEntity entity, Type requestedType)
        {
            if (requestedType==typeof(ITypedEntity))
            {
                return typeof(ITypedEntity);
            }

            var types=entity.AsEntity<ITypedEntity>().Types.Select(t => t.Uri).ToList();

            if (!types.Any()||!_directlyDerivingTypes.ContainsKey(requestedType))
            {
                return requestedType;
            }

            Type bestMatch=requestedType;
            var childTypesToCheck=new Queue<Type>(_directlyDerivingTypes[requestedType]);
            while (childTypesToCheck.Any())
            {
                Type currentParent=childTypesToCheck.Dequeue();

                if (_directlyDerivingTypes.ContainsKey(currentParent))
                {
                    foreach (var child in _directlyDerivingTypes[currentParent])
                    {
                        childTypesToCheck.Enqueue(child);
                    }
                }

                if (_classUris.ContainsKey(currentParent))
                {
                    if (_classUris[currentParent].IsSupersetOf(types))
                    {
                        bestMatch=currentParent;
                    }
                }
            }

            return bestMatch;
        }

        /// <summary>
        /// Sets the currently processed enitty type
        /// and updates inheritance cache
        /// </summary>
        public void Visit(IEntityMapping entityMapping)
        {
            if (!_classUris.ContainsKey(entityMapping.EntityType))
            {
                _classUris.Add(entityMapping.EntityType,new HashSet<Uri>(new AbsoluteUriComparer()));
            }

            AddAsChildOfParentTypes(entityMapping.EntityType);

            _currentClasses=_classUris[entityMapping.EntityType];
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
            _currentClasses.Add(classMapping.Uri);
        }

        private void AddAsChildOfParentTypes(Type entityType)
        {
            foreach (var parentType in entityType.GetImmediateParents(false))
            {
                if (!_directlyDerivingTypes.ContainsKey(parentType))
                {
                    _directlyDerivingTypes.Add(parentType,new HashSet<Type>());
                }

                _directlyDerivingTypes[parentType].Add(entityType);
            }
        }
    }
}