using System.Reflection;

namespace RomanticWeb.Mapping
{
    /// <summary>
    /// Base class for implementations of <see cref="IMappingsRepository"/>, which scan an <see cref="Assembly"/>
    /// </summary>
    public abstract class AssemblyMappingsRepository:MappingsRepositoryBase
    {
        private readonly Assembly _assembly;

        /// <summary>Initializes a new instance of the <see cref="AssemblyMappingsRepository"/> class.</summary>
        /// <param name="assembly">The assembly.</param>
        protected AssemblyMappingsRepository(Assembly assembly)
        {
            _assembly=assembly;
        }

        /// <summary>Gets the source <see cref="System.Reflection.Assembly"/>.</summary>
        protected Assembly Assembly
        {
            get
            {
                return _assembly;
            }
        }
    }
}