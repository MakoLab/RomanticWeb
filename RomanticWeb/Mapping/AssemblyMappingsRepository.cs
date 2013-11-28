using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyMappingsRepository"/> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        protected AssemblyMappingsRepository(Assembly assembly)
        {
            _assembly=assembly;
        }

        /// <summary>
        /// Gets the source <see cref="System.Reflection.Assembly"/>
        /// </summary>
        protected Assembly Assembly
        {
            get
            {
                return _assembly;
            }
        }

        /// <summary>Gets a mapping for an Entity type.</summary>
        /// <typeparam name="TEntity">Entity type, for which mappings is going to be retrieved.</typeparam>
        [return:AllowNull]
        public IEntityMapping MappingFor<TEntity>()
        {
            return (_mappings.ContainsKey(typeof(TEntity))?_mappings[typeof(TEntity)]:null);
        }

        /// <inheritdoc />
        public void RebuildMappings(MappingContext mappingContext)
        {
            _mappings = new Dictionary<Type, IEntityMapping>();
            foreach (var mapping in BuildTypeMappings(mappingContext))
            {
                if (_mappings.ContainsKey(mapping.Item1))
                {
                    throw new MappingException(string.Format("Duplicate mapping for type {0}",mapping.Item1));
                }

                _mappings.Add(mapping.Item1, mapping.Item2);
            }
        }

        /// <summary>
        /// Builds mapping from the current <see cref="Assembly"/>
        /// </summary>
        protected abstract IEnumerable<Tuple<Type,IEntityMapping>> BuildTypeMappings(MappingContext mappingContext);
    }
}