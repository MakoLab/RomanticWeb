using System.Collections.Generic;
using System.Reflection;
using RomanticWeb.Mapping.Providers;

namespace RomanticWeb.Mapping
{
    /// <summary>
    /// Base class for implementations of <see cref="IMappingSource"/>, which scan an <see cref="Assembly"/>
    /// </summary>
    public abstract class AssemblyMappingsSource:IMappingSource
    {
        private readonly Assembly _assembly;

        /// <summary>Initializes a new instance of the <see cref="AssemblyMappingsSource"/> class.</summary>
        /// <param name="assembly">The assembly.</param>
        protected AssemblyMappingsSource(Assembly assembly)
        {
            _assembly=assembly;
        }

        /// <summary>Gets the source <see cref="System.Reflection.Assembly"/>.</summary>
        protected Assembly Assembly { get { return _assembly; } }

        public abstract IEnumerable<IEntityMappingProvider> GetMappingProviders();
    }
}