using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RomanticWeb.Ontologies
{
    /// <summary>Enumerates all built in ontologies.</summary>
    [Flags]
    public enum BuiltInOntologies:uint
    {
        /// <summary>Points to an RDF ontology.</summary>
        RDF=1,

        /// <summary>Points to an RDF Schema ontology.</summary>
        RDFS=1<<1,

        /// <summary>Points to an OWL ontology.</summary>
        OWL=1<<2
    }

    /// <summary>Provides default, built in ontologies.</summary>
    public sealed class DefaultOntologiesProvider:OntologyProviderBase
    {
        private IList<Ontology> _ontologies;
        private IList<BuiltInOntologies> _includedOntologies;

        /// <summary>Creates a default ontology provider with all built in ontologies.</summary>
        public DefaultOntologiesProvider():base()
        {
            _ontologies=new List<Ontology>();
            _includedOntologies=new List<BuiltInOntologies>();
            Include(BuiltInOntologies.RDF|BuiltInOntologies.RDFS|BuiltInOntologies.OWL);
        }

        /// <summary>Creates a default ontology provider with given built in ontologies initialized.</summary>
        /// <param name="ontologyProvider">Ontology provider to be wrapped by this instance.</param>
        public DefaultOntologiesProvider(IOntologyProvider ontologyProvider):this(ontologyProvider,BuiltInOntologies.RDF|BuiltInOntologies.RDFS|BuiltInOntologies.OWL)
        {
        }

        /// <summary>Creates a default ontology provider with given built in ontologies initialized.</summary>
        /// <param name="ontologies">Ontologies to be included int this instance.</param>
        public DefaultOntologiesProvider(BuiltInOntologies ontologies):this()
        {
            Include(ontologies);
        }

        /// <summary>Creates a default ontology provider with given built in ontologies initialized.</summary>
        /// <param name="ontologyProvider">Ontology provider to be wrapped by this instance.</param>
        /// <param name="ontologies">Ontologies to be included int this instance.</param>
        public DefaultOntologiesProvider(IOntologyProvider ontologyProvider,BuiltInOntologies ontologies):base()
        {
            _ontologies=ontologyProvider.Ontologies.ToList();
            _includedOntologies=new List<BuiltInOntologies>();
            Include(ontologies);
        }

        /// <summary>Get ontologies' metadata.</summary>
        public override IEnumerable<Ontology> Ontologies { get { return _ontologies; } }

        /// <summary>Adds another built in ontology into this provider instance.</summary>
        /// <param name="ontologies">Ontologiesto be included in this instance.</param>
        /// <returns>This instanc of the default ontologies provider.</returns>
        public DefaultOntologiesProvider Include(BuiltInOntologies ontologies)
        {
            foreach (BuiltInOntologies ontology in Enum.GetValues(typeof(BuiltInOntologies)))
            {
                if (((ontologies&ontology)==ontology)&&(!_includedOntologies.Contains(ontology)))
                {
                    Ontology ontologyInstance=OntologyFactory.Create(
                        Assembly.GetExecutingAssembly().GetManifestResourceStream(System.String.Format("{0}.{1}.owl",typeof(DefaultOntologiesProvider).Namespace,ontology.ToString())),
                        "application/owl+xml");
                    if (ontologyInstance!=null)
                    {
                        _includedOntologies.Add(ontology);
                        _ontologies.Add(ontologyInstance);
                    }
                }
            }

            return this;
        }
    }
}