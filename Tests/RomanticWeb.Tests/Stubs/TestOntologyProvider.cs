using System;
using System.Collections.Generic;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Tests.Stubs
{
    internal class TestOntologyProvider : OntologyProviderBase
    {
        private readonly bool _includeFoaf;

        public TestOntologyProvider()
            : this(true)
        {
        }

        public TestOntologyProvider(bool includeFoaf)
        {
            _includeFoaf = includeFoaf;
        }

        public override IEnumerable<Ontology> Ontologies
        {
            get
            {
                if (_includeFoaf)
                {
                    yield return new Ontology(
                            new NamespaceSpecification("foaf", new Uri("http://xmlns.com/foaf/0.1/")),
                            new ObjectProperty("knows"),
                            new DatatypeProperty("familyName"),
                            new DatatypeProperty("givenName"),
                            new DatatypeProperty("nick"),
                            new DatatypeProperty("gender"),
                            new Class("Person"),
                            new Class("Agent"),
                            new Class("Document"));
                }

                yield return new Ontology(new NamespaceSpecification("dummy", new Uri("http://example.com")), new DatatypeProperty("test"));
                yield return new Ontology(new NamespaceSpecification("math", new Uri("http://example/maths/")));
                yield return new Ontology(new NamespaceSpecification("magi", new Uri("http://magi/ontology#")));
            }
        }
    }
}