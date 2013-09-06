using System.Collections.Generic;

namespace RomanticWeb.Tests.Stubs
{
    internal class StaticOntologyProvider : IOntologyProvider
    {
        public IEnumerable<Ontology> Ontologies
        {
            get
            {
                yield return new Ontology(new NamespaceSpecification("foaf", "http://xmlns.com/foaf/0.1/"),
                                          new ObjectProperty("knows"),
                                          new DatatypeProperty("familyName"),
                                          new DatatypeProperty("givenName"),
                                          new DatatypeProperty("nick"));
            }
        }
    }
}