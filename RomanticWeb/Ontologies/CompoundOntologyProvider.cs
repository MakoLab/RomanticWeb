using System.Collections.Generic;
using System.Linq;

namespace RomanticWeb.Ontologies
{
    /// <summary>Provides a base functionality for compoung ontology providers.</summary>
    public class CompoundOntologyProvider:OntologyProviderBase
    {
        private IList<IOntologyProvider> _ontologyProviders;

        protected internal CompoundOntologyProvider():base()
        {
            _ontologyProviders=new List<IOntologyProvider>(BuildOntologyProviders());
        }

        public override IEnumerable<Ontology> Ontologies { get { return _ontologyProviders.SelectMany(item => item.Ontologies); } }

        internal IList<IOntologyProvider> OntologyProviders { get { return _ontologyProviders; } }

        /// <summary>Builds lazily an enumeration of ontology providers.</summary>
        /// <returns>Enumeration of ontology providers.</returns>
        protected virtual IEnumerable<IOntologyProvider> BuildOntologyProviders()
        {
            return new IOntologyProvider[0];
        }
    }
}