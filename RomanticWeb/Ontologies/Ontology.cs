using System;
using System.Collections.Generic;
using System.Linq;

namespace RomanticWeb.Ontologies
{
    /// <summary>
    /// Encapsulates metadata about an ontology (like Foaf, Dublin Core, Rdfs, etc.)
    /// </summary>
    public sealed class Ontology
    {
        private readonly NamespaceSpecification _namespace;

        /// <summary>
        /// Creates a new <see cref="Ontology"/> specification
        /// </summary>
        /// <param name="ns">Namespace prefix and base URI</param>
        /// <param name="rdfTerms">A collection of RDF classes and properties</param>
        public Ontology(NamespaceSpecification ns, params RdfTerm[] rdfTerms)
        {
            Properties = rdfTerms.OfType<Property>().Select(p => p.InOntology(this)).ToList();
            _namespace = ns;
            Classes = rdfTerms.OfType<RdfClass>().Select(c => c.InOntology(this)).ToList();
        }

        /// <summary>
        /// Gets the namespace prefix
        /// </summary>
        public string Prefix { get { return _namespace.Prefix; } }

        /// <summary>
        /// Gets the ontology's properties
        /// </summary>
        public IEnumerable<Property> Properties { get; private set; }

        /// <summary>
        /// Gets the ontology's base URI
        /// </summary>
        public Uri BaseUri { get { return _namespace.BaseUri; } }

        /// <summary>
        /// Gets the ontology's classes
        /// </summary>
        public IEnumerable<RdfClass> Classes { get; private set; }

        internal Uri ResolveUri(string rdfTermRelativeUri)
        {
            return new Uri(BaseUri + rdfTermRelativeUri);
        }
    }
}