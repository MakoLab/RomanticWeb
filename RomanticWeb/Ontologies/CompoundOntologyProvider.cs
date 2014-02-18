using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NullGuard;

namespace RomanticWeb.Ontologies
{
    /// <summary>Provides a base functionality for compoung ontology providers.</summary>
    [DebuggerDisplay("{DebuggerString}")]
    [DebuggerTypeProxy(typeof(DebuggerViewProxy))]
    public class CompoundOntologyProvider:OntologyProviderBase
    {
        #region Fields
        private readonly IList<IOntologyProvider> _ontologyProviders;
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
        public override IEnumerable<Ontology> Ontologies { get { return _ontologyProviders.SelectMany(item => item.Ontologies).Distinct(); } }

        /// <summary>Gets a list of ontology proiders stored by this provider.</summary>
        internal IList<IOntologyProvider> OntologyProviders { get { return _ontologyProviders; } }

        private string DebuggerString
        {
            get
            {
                return string.Format("Providers: {0}, Ontolgies: {1}",_ontologyProviders.Count,Ontologies.Count());
            }
        }
        #endregion

        #region Public methods
        /// <summary>Gets a URI from a QName.</summary>
        [return: AllowNull]
        public override Uri ResolveUri(string prefix,string rdfTermName)
        {
            return OntologyProviders.Select(provider => provider.ResolveUri(prefix,rdfTermName)).Where(uri => uri!=null).FirstOrDefault();
        }
        #endregion

        #region Non-public methods
        /// <summary>Builds lazily an enumeration of ontology providers.</summary>
        /// <returns>Enumeration of ontology providers.</returns>
        protected virtual IEnumerable<IOntologyProvider> BuildOntologyProviders()
        {
            return new IOntologyProvider[0];
        }
        #endregion
    
        private class DebuggerViewProxy
        {
            private readonly CompoundOntologyProvider _provider;

            public DebuggerViewProxy(CompoundOntologyProvider provider)
            {
                _provider=provider;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public IList<Ontology> Ontologies
            {
                get
                {
                    return _provider.Ontologies.ToList();
                }
            } 
        }
    }
}