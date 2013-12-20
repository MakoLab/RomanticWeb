using System;
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
        /// <inheritdoc />
        [return: AllowNull]
        public IEntityMapping MappingFor<TEntity>()
        {
            return MappingFor(typeof(TEntity));
        }

        /// <inheritdoc />
        [return: AllowNull]
        public IEntityMapping MappingFor(Type entityType)
        {
            var mappingsForType=_mappingsRepositories.Select(item => item.MappingFor(entityType));
            return mappingsForType.SingleOrDefault(m => m!=null);
        }

        /// <inheritdoc />
        public void RebuildMappings(MappingContext mappingContext)
        {
            foreach (var mappingsRepository in _mappingsRepositories)
            {
                mappingsRepository.RebuildMappings(mappingContext);
            }
        }

        /// <inheritdoc />
        [return: AllowNull]
        public Type MappingFor(Uri classUri)
        {
            return (from mappingsRepository in _mappingsRepositories
                    let mapping=mappingsRepository.MappingFor(classUri)
                    where mapping!=null
                    select mapping).FirstOrDefault();
        }
        #endregion
    }
}