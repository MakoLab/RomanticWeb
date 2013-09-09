using System;
using System.Collections.Generic;

namespace RomanticWeb.Ontologies
{
    public class Ontology
    {
        private readonly NamespaceSpecification _namespace;

        public Ontology(NamespaceSpecification ns, IEnumerable<Property> predicates, IEnumerable<RdfClass> classes)
        {
            Predicates = predicates;
            _namespace = ns;
            Classes = classes;
        }

        public string Prefix { get { return _namespace.Prefix; } }

        public IEnumerable<Property> Predicates { get; private set; }

        public Uri BaseUri { get { return _namespace.BaseUri; } }

        public IEnumerable<RdfClass> Classes { get; private set; }

        public Uri ResolveUri(string rdfTermRelativeUri)
        {
            return new Uri(BaseUri + rdfTermRelativeUri);
        }
    }
}