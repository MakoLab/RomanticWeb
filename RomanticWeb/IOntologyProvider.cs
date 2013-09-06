using System;
using System.Collections.Generic;

namespace RomanticWeb
{
    public interface IOntologyProvider
    {
        IEnumerable<Ontology> Ontologies { get; }
    }

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

    public class Property
    {
        public string PredicateUri { get; private set; }

        public Property(string predicateUri)
        {
            PredicateUri = predicateUri;
        }
    }

    public sealed class DatatypeProperty : Property
    {
        public DatatypeProperty(string predicateUri) : base(predicateUri)
        {
        }
    }

    public sealed class ObjectProperty : Property
    {
        public ObjectProperty(string predicateUri) : base(predicateUri)
        {
        }
    }
}