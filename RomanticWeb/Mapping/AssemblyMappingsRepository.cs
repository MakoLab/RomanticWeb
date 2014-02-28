using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NullGuard;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Mapping
{
    /// <summary>
    /// Base class for implementations of <see cref="IMappingsRepository"/>, which scan an <see cref="Assembly"/>
    /// </summary>
    public abstract class AssemblyMappingsRepository:IMappingsRepository
    {
        private readonly Assembly _assembly;

        private IDictionary<Type,IEntityMapping> _mappings;

        /// <summary>Initializes a new instance of the <see cref="AssemblyMappingsRepository"/> class.</summary>
        /// <param name="assembly">The assembly.</param>
        protected AssemblyMappingsRepository(Assembly assembly)
        {
            _assembly=assembly;
        }

        /// <summary>Gets the source <see cref="System.Reflection.Assembly"/>.</summary>
        protected Assembly Assembly { get { return _assembly; } }

        /// <summary>Gets a mapping for an Entity type.</summary>
        /// <typeparam name="TEntity">Entity type, for which mappings is going to be retrieved.</typeparam>
        [return:AllowNull]
        public IEntityMapping MappingFor<TEntity>()
        {
            return MappingFor(typeof(TEntity));
        }

        /// <inheritdoc />
        [return: AllowNull]
        public IEntityMapping MappingFor(Type entityType)
        {
            return (_mappings.ContainsKey(entityType) ? _mappings[entityType] : null);
        }

        /// <inheritdoc />
        [return: AllowNull]
        public Type MappingFor(Uri classUri)
        {
            return (from mapping in _mappings
                    where mapping.Value.Classes.Any(item => item.Uri.AbsoluteUri==classUri.AbsoluteUri)
                    select mapping.Key).FirstOrDefault();
        }

        /// <inheritdoc />
        public void RebuildMappings(MappingContext mappingContext)
        {
            _mappings = new Dictionary<Type, IEntityMapping>();
            foreach (var mapping in BuildTypeMappings(mappingContext))
            {
                if (_mappings.ContainsKey(mapping.EntityType))
                {
                    throw new MappingException(string.Format("Duplicate mapping for type {0}",mapping.EntityType));
                }

                _mappings.Add(mapping.EntityType, mapping);
            }
        }

        /// <summary>Builds mapping from the current <see cref="Assembly"/>.</summary>
        protected abstract IEnumerable<IEntityMapping> BuildTypeMappings(MappingContext mappingContext);
    }
}