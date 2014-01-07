using System.Collections.Generic;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Tests.Stubs
{
    internal class TestOntologyProvider : OntologyProviderBase
	{
		public override IEnumerable<Ontology> Ontologies
		{
			get
			{
			    yield return new Ontology(new NamespaceSpecification("dummy","http://example.com"),new DatatypeProperty("nick"));

				yield return new Ontology(new NamespaceSpecification("math", "http://example/maths/"));
			}
		}
	}
}