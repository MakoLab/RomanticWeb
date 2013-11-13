using System.Collections.Generic;
using System.Linq;
using NullGuard;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Mapping
{
    /// <summary>Provides a compound-like mappings repository behavior.</summary>
    public sealed class CompoundMappingsRepository:IMappingsRepository
    {
        #region Fields
        private readonly IList<IMappingsRepository> _mappingsRepositories;
        #endregion

        #region Constructors
        /// <summary>Combine the given <paramref name="repositories"/> into a single <see cref="CompoundMappingsRepository"/></summary>
        public CompoundMappingsRepository(IEnumerable<IMappingsRepository> repositories)
        {
            _mappingsRepositories=new List<IMappingsRepository>(repositories);
        }
        #endregion

        #region Properties
        /// <summary>Gets a list of mappings repositories.</summary>
        internal IList<IMappingsRepository> MappingsRepositories { get { return _mappingsRepositories; } }
        #endregion

        #region Pulic methods
        /// <summary>Gets a mapping for an Entity type.</summary>
        /// <typeparam name="TEntity">Entity type, for which mappings is going to be retrieved.</typeparam>
        [return: AllowNull]
        public IEntityMapping MappingFor<TEntity>()
        {
            var mappingsForType=_mappingsRepositories.Select(item => item.MappingFor<TEntity>());
            return mappingsForType.SingleOrDefault(m => m!=null);
        }

        public void RebuildMappings(MappingContext mappingContext)
        {
            foreach (var mappingsRepository in _mappingsRepositories)
            {
                mappingsRepository.RebuildMappings(mappingContext);
            }
        }

        #endregion
    }
}