using System;
using System.Collections.Generic;

namespace RomanticWeb
{
    public class Ontology
    {
        private readonly NamespaceSpecification _namespace;

        public Ontology(NamespaceSpecification ns, params Property[] predicates)
        {
            Predicates = predicates;
            _namespace = ns;
        }

        public string Prefix { get { return _namespace.Prefix; } }

        public IEnumerable<Property> Predicates { get; private set; }

        public Uri BaseUri { get { return _namespace.BaseUri; } }
    }
}