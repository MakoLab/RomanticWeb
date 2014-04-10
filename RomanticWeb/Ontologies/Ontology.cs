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
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    [DebuggerTypeProxy(typeof(DebuggerDisplayProxy))]
    public sealed class Ontology
    {
        private readonly NamespaceSpecification _namespace;
        private readonly string _displayName=String.Empty;

        /// <summary>Creates a new <see cref="Ontology"/> specification.</summary>
        /// <param name="prefix">Prefix of the ontology's base Uri.</param>
        /// <param name="baseUri">Ontology base Uri.</param>
        /// <param name="terms">A collection of RDF classes and properties</param>
        public Ontology(string prefix,Uri baseUri,params Term[] terms):this(String.Empty,prefix,baseUri,terms)
        {
        }

        /// <summary>Creates a new <see cref="Ontology"/> specification.</summary>
        /// <param name="displayName">Display name of the ontology.</param>
        /// <param name="prefix">Prefix of the ontology's base Uri.</param>
        /// <param name="baseUri">Ontology base Uri.</param>
        /// <param name="terms">A collection of RDF classes and properties</param>
        public Ontology([AllowNull] string displayName,string prefix,Uri baseUri,params Term[] terms):this(displayName,new NamespaceSpecification(prefix,baseUri),terms)
        {
        }

        /// <summary>Creates a new <see cref="Ontology"/> specification.</summary>
        /// <param name="namespace">Namespace prefix and base URI</param>
        /// <param name="terms">A collection of RDF classes and properties</param>
        public Ontology(NamespaceSpecification @namespace,params Term[] terms):this(String.Empty,@namespace,terms)
        {
        }

        /// <summary>Creates a new <see cref="Ontology"/> specification.</summary>
        /// <param name="displayName">Display name of the ontology.</param>
        /// <param name="namespace">Namespace prefix and base URI</param>
        /// <param name="terms">A collection of RDF classes and properties</param>
        public Ontology([AllowNull] string displayName,NamespaceSpecification @namespace,params Term[] terms)
        {
            _displayName=(displayName??String.Empty);
            Properties=terms.OfType<Property>().Select(property => property.InOntology(this)).ToList();
            _namespace=@namespace;
            Classes=terms.OfType<Class>().Select(@class => @class.InOntology(this)).ToList();
        }

        /// <summary>Gets the namespace prefix.</summary>
        public string Prefix { get { return _namespace.Prefix; } }

        /// <summary>Gets the display name.</summary>
        /// <remarks>This property is usually fed with dc:title or rdfs:label property.</remarks>
        public string DisplayName { get { return _displayName; } }

        /// <summary>Gets the ontology's base URI.</summary>
        public Uri BaseUri { get { return _namespace.BaseUri; } }

        /// <summary>Gets the ontology's properties.</summary>
        public IEnumerable<Property> Properties { get; private set; }

        /// <summary>Gets the ontology's classes.</summary>
        public IEnumerable<Class> Classes { get; private set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get
            {
                return string.IsNullOrWhiteSpace(DisplayName)?BaseUri.ToString():DisplayName;
            }
        }

        /// <summary>Tests for equality of two ontologies.</summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns><b>true</b> if both ontologies has equal namespaces; otherwise, <b>false</b>.</returns>
        public static bool operator==([AllowNull] Ontology left,[AllowNull] Ontology right)
        {
            return Equals(left,right);
        }

        /// <summary>Tests for inequality of two ontologies.</summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns><b>true</b> if both ontologies has different namespaces; otherwise, <b>false</b>.</returns>
        public static bool operator!=([AllowNull] Ontology left,[AllowNull] Ontology right)
        {
            return !Equals(left,right);
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="operand">Type: <see cref="System.Object" />
        /// The object to compare with the current object.</param>
        /// <returns>Type: <see cref="System.Boolean" />
        /// <b>true</b> if the specified object is equal to the current object; otherwise, <b>false</b>.</returns>
        public override bool Equals([AllowNull] object operand)
        {
            if (ReferenceEquals(null,operand)) { return false; }
            if (ReferenceEquals(this,operand)) { return true; }
            return (operand is Ontology)&&(Equals((Ontology)operand));
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>Type: <see cref="System.Int32" />
        /// A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return (_namespace!=null?_namespace.GetHashCode():0);
        }

        /// <summary>Determines whether the specified ontology is equal to the ontology object.</summary>
        /// <param name="operand">Type: <see cref="Ontology" />
        /// The ontology to compare with the current ontology.</param>
        /// <returns>Type: <see cref="System.Boolean" />
        /// <b>true</b> if the specified ontology hase equal namespace with current ontology; otherwise, <b>false</b>.</returns>
        private bool Equals([AllowNull] Ontology operand)
        {
            return Equals(_namespace,operand._namespace);
        }

        private class DebuggerDisplayProxy
        {
            private readonly Ontology _ontology;

            public DebuggerDisplayProxy(Ontology ontology)
            {
                _ontology=ontology;
            }

            public string Prefix
            {
                get
                {
                    return _ontology.Prefix;
                }
            }

            public Uri Uri
            {
                get
                {
                    return _ontology.BaseUri;
                }
            }

            public IEnumerable<Class> Classes
            {
                get
                {
                    return _ontology.Classes.OrderBy(c=>c.ClassName).ToList();
                }
            }

            public IEnumerable<Property> Properties
            {
                get
                {
                    return _ontology.Properties.OrderBy(c => c.PropertyName).ToList();
                }
            }
        }
    }
}