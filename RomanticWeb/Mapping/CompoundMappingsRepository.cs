using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Mapping
{
    /// <summary>Provides a compound-like mappings repository behavior.</summary>
    public sealed class CompoundMappingsRepository:IMappingsRepository
    {
        #region Fields
        private IOntologyProvider _ontologyProvider;
        private IList<IMappingsRepository> _mappingsRepositories;
        #endregion

        #region Constructors
        /// <summary>Default parameterles constrctor.</summary>
        public CompoundMappingsRepository()
        {
            ObservableCollection<IMappingsRepository> mappingsRepositories=new ObservableCollection<IMappingsRepository>();
            mappingsRepositories.CollectionChanged+=OnMappingsRepositoriesCollectionChanged;
            _mappingsRepositories=mappingsRepositories;
        }
        #endregion

        #region Events
        internal event NotifyCollectionChangedEventHandler CollectionChanged;
        #endregion

        #region Properties
        /// <summary>Gets or sets an ontology provider to be used by this mappings repository to resolve namespace prefixes.</summary>
        IOntologyProvider IMappingsRepository.OntologyProvider
        {
            get
            {
                return _ontologyProvider;
            }

            set
            {
                if ((_ontologyProvider=value) is CompoundOntologyProvider)
                {
                    ((CompoundOntologyProvider)_ontologyProvider).CollectionChanged+=OnOntologyProviderCollectionChanged;
                }

                OnOntologyProviderCollectionChanged(_ontologyProvider,new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        /// <summary>Gets a list of mappings repositories.</summary>
        internal IList<IMappingsRepository> MappingsRepositories { get { return _mappingsRepositories; } }
        #endregion

        #region Pulic methods
        /// <summary>Gets a mapping for an Entity type.</summary>
        /// <typeparam name="TEntity">Entity type, for which mappings is going to be retrieved.</typeparam>
        public IEntityMapping MappingFor<TEntity>()
        {
            return _mappingsRepositories.Select(item => item.MappingFor<TEntity>()).FirstOrDefault();
        }
        #endregion

        #region Private methods
        private void OnMappingsRepositoriesCollectionChanged(object sender,NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (IMappingsRepository mappingsRepository in e.NewItems)
                    {
                        mappingsRepository.OntologyProvider=_ontologyProvider;
                    }

                    break;
            }

            if (CollectionChanged!=null)
            {
                CollectionChanged(this,e);
            }
        }

        /// <summary>Forces mappings repositories to rebind their mappings as the ontology provider has changed.</summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void OnOntologyProviderCollectionChanged(object sender,NotifyCollectionChangedEventArgs e)
        {
            foreach (IMappingsRepository mappingsRepository in _mappingsRepositories)
            {
                mappingsRepository.OntologyProvider=_ontologyProvider;
            }
        }
        #endregion
    }
}