using System.Collections.Generic;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Tests.Stubs
{
    internal class TestOntologyProvider : OntologyProviderBase
    {
        private bool _includeFoaf=true;
        public TestOntologyProvider()
        {
        }

        public TestOntologyProvider(bool includeFoaf)
        {
            _includeFoaf=includeFoaf;
        }

        public override IEnumerable<Ontology> Ontologies
        {
            get
            {
                if (_includeFoaf)
                {
                    yield return new Ontology(
                            new NamespaceSpecification("foaf","http://xmlns.com/foaf/0.1/"),
                            new ObjectProperty("knows"),
                            new DatatypeProperty("familyName"),
                            new DatatypeProperty("givenName"),
                            new DatatypeProperty("nick"),
                            new Class("Person"),
                            new Class("Agent"),
                            new Class("Document"));
                }

                yield return new Ontology(new NamespaceSpecification("dummy","http://example.com"),new DatatypeProperty("test"));

                yield return new Ontology(new NamespaceSpecification("math", "http://example/maths/"));
            }
        }
    }
}