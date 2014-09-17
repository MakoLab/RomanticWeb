using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace RomanticWeb.Ontologies
{
    /// <summary>Enumerates all built in ontologies.</summary>
    [Flags]
    public enum BuiltInOntologies : uint
    {
        /// <summary>Points to an Resource Description Framework ontology.</summary>
        RDF = 1,

        /// <summary>Points to an RDF Schema ontology.</summary>
        RDFS = 1 << 1,

        /// <summary>Points to a Web Ontology Language ontology.</summary>
        OWL = 1 << 2,

        /// <summary>Points to a Simple Knowledge Organization System ontology.</summary>
        SKOS = 1 << 3,

        /// <summary>Points to a Dublin Core ontology.</summary>
        DC = 1 << 4,

        /// <summary>Points to a Dublin Core Terms ontology.</summary>
        DCTerms = 1 << 5,

        /// <summary>Points to a Dublin Core Abstract Model ontology.</summary>
        DCAM = 1 << 6,

        /// <summary>Points to a Dublin Core Metadata Initiatie Type vocabulary.</summary>
        DCMIType = 1 << 7,

        /// <summary>Points to a Friend of a Friend vocabulary.</summary>
        FOAF = 1 << 8,

        /// <summary>Points to a Schema.org vocabulary.</summary>
        Schema = 1 << 9,

        /// <summary>Points to a GoodRelations ontology.</summary>
        GR = 1 << 10,

        /// <summary>Points to a Semantically-Interlinked Online Communities ontology.</summary>
        SIOC = 1 << 11,

        /// <summary>Points to a WGS84 Geo Positioning: an RDF vocabulary.</summary>
        GEO = 1 << 12,

        /// <summary>Points to the W3C Linked Data Platform vocabulary.</summary>
        LDP = 1 << 13,

        /// <summary>Points to the W3C content description vocabulary.</summary>
        CNT = 1 << 14,

        /// <summary>Points to the SPIN Modeling Vocabulary.</summary>
        SPIN = 1 << 15,

        /// <summary>Points to the SPIN SPARQL Syntax.</summary>
        SP = 1 << 16
    }

    /// <summary>Provides default, built in ontologies.</summary>
    [DebuggerDisplay("Ontologies count = {_ontologies.Count}")]
    [DebuggerTypeProxy(typeof(DebuggerViewProxy))]
    public sealed class DefaultOntologiesProvider : OntologyProviderBase
    {
        /// <summary>Provides map of supported OWL serialization and their file extensions.</summary>
        public static readonly IDictionary<string, string> OwlSerializationExtensions = new Dictionary<string, string>
        {
            { "application/rdf+xml", "rdf" },
            { "application/owl+xml", "owl" }
        };

        private IList<Ontology> _ontologies;
        private IList<BuiltInOntologies> _includedOntologies;
        private OntologyFactory _ontologyFactory;

        /// <summary>Creates a default ontology provider with all built in ontologies.</summary>
        public DefaultOntologiesProvider()
        {
            _ontologyFactory = new OntologyFactory();
            _ontologies = new List<Ontology>();
            _includedOntologies = new List<BuiltInOntologies>();
            Include(BuiltInOntologies.RDF |
                BuiltInOntologies.RDFS |
                BuiltInOntologies.OWL |
                BuiltInOntologies.SKOS |
                BuiltInOntologies.DC |
                BuiltInOntologies.DCTerms |
                BuiltInOntologies.DCAM |
                BuiltInOntologies.DCMIType |
                BuiltInOntologies.FOAF |
                BuiltInOntologies.Schema |
                BuiltInOntologies.SIOC |
                BuiltInOntologies.GEO |
                BuiltInOntologies.LDP |
                BuiltInOntologies.CNT |
                BuiltInOntologies.SPIN |
                BuiltInOntologies.SP);
        }

        /// <summary>Creates a default ontology provider with given built in ontologies initialized.</summary>
        /// <param name="ontologies">Ontologies to be included int this instance.</param>
        public DefaultOntologiesProvider(BuiltInOntologies ontologies)
            : this()
        {
            Include(ontologies);
        }

        /// <summary>Get ontologies' metadata.</summary>
        public override IEnumerable<Ontology> Ontologies { get { return _ontologies; } }

        /// <summary>Adds another built in ontology into this provider instance.</summary>
        /// <param name="ontologies">Ontologiesto be included in this instance.</param>
        /// <returns>This instance of the default ontologies provider.</returns>
        public DefaultOntologiesProvider Include(BuiltInOntologies ontologies)
        {
            foreach (BuiltInOntologies ontology in Enum.GetValues(typeof(BuiltInOntologies)))
            {
                if (((ontologies & ontology) == ontology) && (!_includedOntologies.Contains(ontology)))
                {
                    string resourceName = System.String.Format("{0}.{1}.", typeof(DefaultOntologiesProvider).Namespace, ontology.ToString());
                    resourceName = (from manifestResourceName in Assembly.GetExecutingAssembly().GetManifestResourceNames()
                                    where manifestResourceName.StartsWith(resourceName)
                                    select manifestResourceName).FirstOrDefault();
                    if (System.String.IsNullOrEmpty(resourceName))
                    {
                        throw new System.IO.FileNotFoundException(System.String.Format("No embedded ontology stream found for '{0}'.", ontology.ToString()));
                    }

                    Ontology ontologyInstance = _ontologyFactory.Create(
                        Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName),
                        "application/" + (resourceName.EndsWith(".owl") ? "owl" : "rdf") + "+xml");
                    if (ontologyInstance != null)
                    {
                        _includedOntologies.Add(ontology);
                        _ontologies.Add(ontologyInstance);
                    }
                }
            }

            return this;
        }

        /// <summary>Includes an Resource Description Framework ontology.</summary>
        /// <returns>This instance of the default ontologies provider.</returns>
        public DefaultOntologiesProvider WithRDF()
        {
            return Include(BuiltInOntologies.RDF);
        }

        /// <summary>Includes an RDF Schema ontology.</summary>
        /// <returns>This instance of the default ontologies provider.</returns>
        public DefaultOntologiesProvider WithRDFS()
        {
            return Include(BuiltInOntologies.RDFS);
        }

        /// <summary>Includes a Web Ontology Language ontology.</summary>
        /// <returns>This instance of the default ontologies provider.</returns>
        public DefaultOntologiesProvider WithOWL()
        {
            return Include(BuiltInOntologies.OWL);
        }

        /// <summary>Includes a Simple Knowledge Organization System ontology.</summary>
        /// <returns>This instance of the default ontologies provider.</returns>
        public DefaultOntologiesProvider WithSKOS()
        {
            return Include(BuiltInOntologies.SKOS);
        }

        /// <summary>Includes a Dublin Core ontology.</summary>
        /// <returns>This instance of the default ontologies provider.</returns>
        public DefaultOntologiesProvider WithDC()
        {
            return Include(BuiltInOntologies.DC);
        }

        /// <summary>Includes a Dublin Core Terms ontology.</summary>
        /// <returns>This instance of the default ontologies provider.</returns>
        public DefaultOntologiesProvider WithDCTerms()
        {
            return Include(BuiltInOntologies.DCTerms);
        }

        /// <summary>Includes a Dublin Core Abstraction Model ontology.</summary>
        /// <returns>This instance of the default ontologies provider.</returns>
        public DefaultOntologiesProvider WithDCAM()
        {
            return Include(BuiltInOntologies.DCAM);
        }

        /// <summary>Includes a Dublin Core Metadata Initiative Type vocabulary.</summary>
        /// <returns>This instance of the default ontologies provider.</returns>
        public DefaultOntologiesProvider WithDCMIType()
        {
            return Include(BuiltInOntologies.DCMIType);
        }

        /// <summary>Includes a Friend of a Friend vocabulary.</summary>
        /// <returns>This instance of the default ontologies provider.</returns>
        public DefaultOntologiesProvider WithFOAF()
        {
            return Include(BuiltInOntologies.FOAF);
        }

        /// <summary>Includes a Schema.org vocabulary.</summary>
        /// <returns>This instance of the default ontologies provider.</returns>
        public DefaultOntologiesProvider WithSchema()
        {
            return Include(BuiltInOntologies.Schema);
        }

        /// <summary>Includes a GoodRelations ontology.</summary>
        /// <returns>This instance of the default ontologies provider.</returns>
        public DefaultOntologiesProvider WithGR()
        {
            return Include(BuiltInOntologies.GR);
        }

        /// <summary>Includes a Semantically-Interlinked Online Communities ontology.</summary>
        /// <returns>This instance of the default ontologies provider.</returns>
        public DefaultOntologiesProvider WithSIOC()
        {
            return Include(BuiltInOntologies.SIOC);
        }

        /// <summary>Includes a WGS84 Geo Positioning: an RDF vocabulary.</summary>
        /// <returns>This instance of the default ontologies provider.</returns>
        public DefaultOntologiesProvider WithGEO()
        {
            return Include(BuiltInOntologies.GEO);
        }

        /// <summary>Includes the W3C Linked Data Platform vocabulary.</summary>
        /// <returns>This instance of the default ontologies provider.</returns>
        public DefaultOntologiesProvider WithLDP()
        {
            return Include(BuiltInOntologies.LDP);
        }

        /// <summary>Includes the W3C content description vocabulary.</summary>
        /// <returns>This instance of the default ontologies provider.</returns>
        public DefaultOntologiesProvider WithCNT()
        {
            return Include(BuiltInOntologies.CNT);
        }

        /// <summary>Includes the SPIN Modeling Vocabulary.</summary>
        /// <returns>This instance of the default ontologies provider.</returns>
        public DefaultOntologiesProvider WithSPIN()
        {
            return Include(BuiltInOntologies.SPIN);
        }

        /// <summary>Includes the SPIN SPARQL Syntax.</summary>
        /// <returns>This instance of the default ontologies provider.</returns>
        public DefaultOntologiesProvider WithSP()
        {
            return Include(BuiltInOntologies.SP);
        }

        private class DebuggerViewProxy
        {
            private readonly DefaultOntologiesProvider _provider;

            public DebuggerViewProxy(DefaultOntologiesProvider provider)
            {
                _provider = provider;
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