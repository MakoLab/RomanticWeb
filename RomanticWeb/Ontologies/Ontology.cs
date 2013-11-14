using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NullGuard;

namespace RomanticWeb.Ontologies
{
    /// <summary>
    /// Encapsulates metadata about an ontology (like Foaf, Dublin Core, Rdfs, etc.)
    /// </summary>
    [DebuggerDisplay("Ontology {Prefix} (<{BaseUri}>)")]
    public sealed class Ontology
    {
        private readonly NamespaceSpecification _namespace;

		/// <summary>Creates a new <see cref="Ontology"/> specification.</summary>
		/// <param name="prefix">Prefix of the ontology's base Uri.</param>
		/// <param name="baseUri">Ontology base Uri.</param>
		/// <param name="terms">A collection of RDF classes and properties</param>
		public Ontology(string prefix,string baseUri,params Term[] terms):this(new NamespaceSpecification(prefix,baseUri),terms)
		{
		}

		/// <summary>Creates a new <see cref="Ontology"/> specification.</summary>
        /// <param name="ns">Namespace prefix and base URI</param>
        /// <param name="terms">A collection of RDF classes and properties</param>
        public Ontology(NamespaceSpecification ns, params Term[] terms)
        {
            Properties = terms.OfType<Property>().Select(p => p.InOntology(this)).ToList();
            _namespace = ns;
            Classes = terms.OfType<Class>().Select(c => c.InOntology(this)).ToList();
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
        public IEnumerable<Class> Classes { get; private set; }

#pragma warning disable 1591
        public static bool operator ==([AllowNull] Ontology left, [AllowNull] Ontology right)
        {
			return Equals(left,right);
        }

		public static bool operator!=([AllowNull] Ontology left,[AllowNull] Ontology right)
        {
			return !Equals(left,right);
        }

        public override bool Equals([AllowNull] object obj)
        {
			if (ReferenceEquals(null,obj)) { return false; }
			if (ReferenceEquals(this,obj)) { return true; }
			return obj is Ontology&&Equals((Ontology)obj);
        }

        public override int GetHashCode()
        {
			return (_namespace!=null?_namespace.GetHashCode():0);
        }

        private bool Equals([AllowNull] Ontology other)
        {
			return Equals(_namespace,other._namespace);
        }
#pragma warning restore
    }
}