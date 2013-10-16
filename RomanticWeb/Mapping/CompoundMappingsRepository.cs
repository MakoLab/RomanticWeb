using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using NullGuard;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Mapping
{
    /// <summary>Provides a compound-like mappings repository behavior.</summary>
    public sealed class CompoundMappingsRepository:IMappingsRepository
    {
        #region Fields
        private IList<IMappingsRepository> _mappingsRepositories;
        #endregion

        #region Constructors
        /// <summary>Default parameterles constrctor.</summary>
        public CompoundMappingsRepository()
        {
            ObservableCollection<IMappingsRepository> mappingsRepositories=new ObservableCollection<IMappingsRepository>();
            _mappingsRepositories=mappingsRepositories;
        }
        #endregion

        #region Events
        internal event NotifyCollectionChangedEventHandler CollectionChanged;
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

        public void RebuildMappings(IOntologyProvider ontologyProvider)
        {
            foreach (var mappingsRepository in _mappingsRepositories)
            {
                mappingsRepository.RebuildMappings(ontologyProvider);
            }
        }

        #endregion
    }
}