using System;
using System.Collections.Generic;
using System.Reflection;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Mapping
{
    /// <summary>
    /// Base class for implementations of <see cref="IMappingsRepository"/>, which scan an <see cref="Assembly"/>
    /// </summary>
    public abstract class AssemblyMappingsRepository:IMappingsRepository
    {
        private readonly Assembly _assembly;

        private IDictionary<Type,IEntityMapping> _mappings;

        protected AssemblyMappingsRepository(Assembly assembly)
        {
            _assembly=assembly;
        }

        protected Assembly Assembly
        {
            get
            {
                return _assembly;
            }
        }

        /// <summary>Gets a mapping for an Entity type.</summary>
        /// <typeparam name="TEntity">Entity type, for which mappings is going to be retrieved.</typeparam>
        public IEntityMapping MappingFor<TEntity>()
        {
            var entityType=typeof(TEntity);
            if (_mappings.ContainsKey(entityType))
            {
                return _mappings[entityType];
            }
            
            throw new MappingException(string.Format("No mapping found for type '{0}'",entityType));
        }

        public void RebuildMappings(IOntologyProvider ontologyProvider)
        {
            _mappings = new Dictionary<Type, IEntityMapping>();
            foreach (var mapping in BuildTypeMappings(ontologyProvider))
            {
                if (_mappings.ContainsKey(mapping.Item1))
                {
                    throw new MappingException(string.Format("Duplicate mapping for type {0}",mapping.Item1));
                }

                _mappings.Add(mapping.Item1, mapping.Item2);
            }
        }

        protected abstract IEnumerable<Tuple<Type,IEntityMapping>> BuildTypeMappings(IOntologyProvider ontologyProvider);
    }
}