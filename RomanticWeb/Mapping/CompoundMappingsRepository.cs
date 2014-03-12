using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Anotar.NLog;
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
            IList<IEntityMapping> result=(
                from mappingRepository in _mappingsRepositories
                let mapping=mappingRepository.MappingFor(entityType)
                where mapping!=null
                select mapping).ToList();
            if (result.Count>1)
            {
                return new EntityMapping(entityType,result.SelectMany(item => item.Classes),result.SelectMany(item => item.Properties));
            }
            else if (result.Count>0)
            {
                return result[0];
            }

            return null;
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

        /// <inheritdoc />
        [return: AllowNull]
        public PropertyInfo MappingForProperty(Uri predicateUri)
        {
            return (from repository in _mappingsRepositories
                    let mapping=repository.MappingForProperty(predicateUri)
                    where mapping!=null
                    select mapping).FirstOrDefault();
        }

        #endregion
    }
}