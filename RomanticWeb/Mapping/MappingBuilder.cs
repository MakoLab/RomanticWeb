using System.Collections.Generic;
using System.Reflection;
using NullGuard;
using RomanticWeb.Mapping.Sources;

namespace RomanticWeb.Mapping
{
    /// <summary>
    /// Builder for registering mapping repositories with <see cref="IEntityContextFactory"/>
    /// </summary>
    [NullGuard(ValidationFlags.All)]
    public sealed class MappingBuilder
    {
        private readonly IList<MappingProviderSourceMeta> _sources = new List<MappingProviderSourceMeta>();

        /// <summary>
        /// Allows registering attribute mappings
        /// </summary>
        public MappingFrom Attributes
        {
            get
            {
                return new MappingFromAttributes(this);
            }
        }

        /// <summary>
        /// Allows registering fluent mappings
        /// </summary>
        public MappingFrom Fluent
        {
            get
            {
                return new MappingFromFluent(this);
            }
        }

        internal IEnumerable<MappingProviderSourceMeta> Sources
        {
            get
            {
                return _sources;
            }
        }

        /// <summary>
        /// Registers both fluent and attrbiute mappings from an assembly
        /// </summary>
        public void FromAssemblyOf<T>()
        {
            Fluent.FromAssemblyOf<T>();
            Attributes.FromAssemblyOf<T>();
        }

        /// <summary>
        /// Registers both fluent and attrbiute mappings from an assembly
        /// </summary>
        public void FromAssembly(Assembly assembly)
        {
            Fluent.FromAssembly(assembly);
            Attributes.FromAssembly(assembly);
        }

        internal void AddMapping<TMappingRepository>(Assembly mappingAssembly, TMappingRepository mappingsRepository)
            where TMappingRepository : IMappingProviderSource
        {
            _sources.Add(new MappingProviderSourceMeta(mappingAssembly, mappingsRepository));
        }

        internal struct MappingProviderSourceMeta
        {
            private readonly Assembly _mappingAssembly;
            private readonly IMappingProviderSource _mappingsRepository;

            public MappingProviderSourceMeta(Assembly mappingAssembly, IMappingProviderSource mappingsRepository)
            {
                _mappingAssembly = mappingAssembly;
                _mappingsRepository = mappingsRepository;
            }

            public IMappingProviderSource Repository
            {
                get
                {
                    return _mappingsRepository;
                }
            }

            public string Name
            {
                get
                {
                    return string.Format("{0} from assembly {1}", Repository.GetType().Name, _mappingAssembly.FullName);
                }
            }
        }
    }
}