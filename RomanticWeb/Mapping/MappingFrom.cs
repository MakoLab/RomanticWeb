using System.Reflection;

namespace RomanticWeb.Mapping
{
    /// <summary>
    /// Implement to allow adding custom mapping repositories to <see cref="IEntityContextFactory"/>
    /// </summary>
    public abstract class MappingFrom
    {
        private readonly MappingBuilder _mappingBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingFrom"/> class.
        /// </summary>
        /// <param name="mappingBuilder">The mapping builder.</param>
        protected MappingFrom(MappingBuilder mappingBuilder)
        {
            _mappingBuilder=mappingBuilder;
        }

        /// <summary>
        /// Gets the mapping builder.
        /// </summary>
        protected MappingBuilder MappingBuilder
        {
            get
            {
                return _mappingBuilder;
            }
        }

        /// <summary>
        /// Registers a mapping repository, which will look for mappings in the given <see cref="Assembly"/>
        /// </summary>
        public abstract void FromAssembly(Assembly mappingAssembly);

        /// <summary>
        /// Registers a mapping repository, which will look for mappings in the <see cref="Assembly"/> of the given entity type
        /// </summary>
        public void FromAssemblyOf<TEntity>()
        {
            FromAssembly(typeof(TEntity).Assembly);
        }
    }
}