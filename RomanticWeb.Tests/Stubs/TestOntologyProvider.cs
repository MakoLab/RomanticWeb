using System.Collections.Generic;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Tests.Stubs
{
	internal class TestOntologyProvider : IOntologyProvider
	{
		public IEnumerable<Ontology> Ontologies
		{
			get
			{
				yield return new Ontology(new NamespaceSpecification("foaf", "http://xmlns.com/foaf/0.1/"),
							              new ObjectProperty("knows"),
							              new DatatypeProperty("familyName"),
							              new DatatypeProperty("givenName"),
							              new DatatypeProperty("nick"),
							              new RdfClass("Person"),
							              new RdfClass("Agent"),
							              new RdfClass("Document")
					);

				yield return new Ontology(new NamespaceSpecification("dummy", "http://example.com"),
							              new DatatypeProperty("nick"));

				yield return new Ontology(new NamespaceSpecification("math", "http://example/maths/"));
			}
		}
	}
}