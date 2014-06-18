using System;
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
        private readonly MappingsRepository _mappingsRepository;

        internal MappingBuilder(MappingsRepository mappingsRepository)
        {
            _mappingsRepository=mappingsRepository;
        }

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

        /// <summary>
        /// Registers both fluent and attrbiute mappings from an assembly
        /// </summary>
        public void FromAssemblyOf<T>()
        {
            Fluent.FromAssemblyOf<T>();
            Attributes.FromAssemblyOf<T>();
        }

        public void FromAssembly(Assembly assembly)
        {
            Fluent.FromAssembly(assembly);
            Attributes.FromAssembly(assembly);
        }

        internal void AddMapping<TMappingRepository>(Assembly mappingAssembly,TMappingRepository mappingsRepository)
            where TMappingRepository:IMappingProviderSource
        {
            _mappingsRepository.AddSource(mappingAssembly,mappingsRepository);
        }
    }
}