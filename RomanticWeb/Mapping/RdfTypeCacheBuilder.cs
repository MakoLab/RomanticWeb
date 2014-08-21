using System;
using System.Collections.Generic;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Mapping
{
    internal class RdfTypeCacheBuilder : Visitors.IMappingModelVisitor
    {
        private readonly IRdfTypeCache _rdfTypeCache;
        private readonly IDictionary<Type, IList<IClassMapping>> _classMappings;
        private IList<IClassMapping> _currentClasses;

        public RdfTypeCacheBuilder(IRdfTypeCache rdfTypeCache)
        {
            _classMappings = new Dictionary<Type, IList<IClassMapping>>();
            _rdfTypeCache = rdfTypeCache;
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

            _rdfTypeCache.Add(entityMapping.EntityType, _classMappings[entityMapping.EntityType]);

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
    }
}