using System.Collections.Generic;
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
        internal CompoundOntologyProvider(params IOntologyProvider[] ontologyProviders)
        {
            _ontologyProviders=new List<IOntologyProvider>(ontologyProviders);
        }

        internal CompoundOntologyProvider(IEnumerable<IOntologyProvider> ontologyProviders):this(ontologyProviders.ToArray())
        {
        }
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
        #endregion
    }
}