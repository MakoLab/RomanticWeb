using System.Collections.Generic;
using System.Reflection;
using RomanticWeb.Mapping.Providers;

namespace RomanticWeb.Mapping.Sources
{
    /// <summary>
    /// Base class for implementations of <see cref="IMappingProviderSource" />, which scan an <see cref="Assembly" />
    /// </summary>
    public abstract class AssemblyMappingsSource:IMappingProviderSource
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

        /// <summary>
        /// Gets the mapping providers from the <see cref="AssemblyMappingsSource.Assembly"/>.
        /// </summary>
        public abstract IEnumerable<IEntityMappingProvider> GetMappingProviders();
    }
}