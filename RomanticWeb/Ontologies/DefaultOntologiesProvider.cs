using System.Collections.Generic;
using System.Linq;

namespace RomanticWeb.Ontologies
{
    internal sealed class DefaultOntologiesProvider : IOntologyProvider
    {
        private readonly IList<Ontology> _ontologies;

        public DefaultOntologiesProvider(IOntologyProvider ontologyProvider)
        {
            _ontologies = ontologyProvider.Ontologies.Union(DefaultOntologies).ToList();
        }

        private static IEnumerable<Ontology> DefaultOntologies
        {
            get
            {
                yield return new Ontology(
                    new NamespaceSpecification("rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#"),
                    new ObjectProperty("type")
                );
            }
        }

        public IEnumerable<Ontology> Ontologies
        {
            get { return _ontologies; }
        }
    }
}