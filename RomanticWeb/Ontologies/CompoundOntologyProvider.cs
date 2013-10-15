using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace RomanticWeb.Ontologies
{
    /// <summary>Provides a base functionality for compoung ontology providers.</summary>
    public class CompoundOntologyProvider:OntologyProviderBase
    {
        #region Fields
        private IList<IOntologyProvider> _ontologyProviders;
        #endregion

        #region Constructors
        /// <summary>Default parameterles constructor.</summary>
        protected internal CompoundOntologyProvider():base()
        {
            ObservableCollection<IOntologyProvider> ontologyProviders=new ObservableCollection<IOntologyProvider>(BuildOntologyProviders());
            ontologyProviders.CollectionChanged+=OnOntologyProvidersColletionChanged;
            _ontologyProviders=ontologyProviders;
        }
        #endregion

        #region Events
        internal event NotifyCollectionChangedEventHandler CollectionChanged;
        #endregion

        #region Properties
        /// <summary>Get ontologies' metadata.</summary>
        public override IEnumerable<Ontology> Ontologies { get { return _ontologyProviders.SelectMany(item => item.Ontologies); } }

        /// <summary>Gets a list of ontology proiders stored by this provider.</summary>
        internal IList<IOntologyProvider> OntologyProviders { get { return _ontologyProviders; } }
        #endregion

        #region Non-public methods
        /// <summary>Builds lazily an enumeration of ontology providers.</summary>
        /// <returns>Enumeration of ontology providers.</returns>
        protected virtual IEnumerable<IOntologyProvider> BuildOntologyProviders()
        {
            return new IOntologyProvider[0];
        }

        private void OnOntologyProvidersColletionChanged(object sender,NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged!=null)
            {
                CollectionChanged(this,e);
            }
        }
        #endregion
    }
}