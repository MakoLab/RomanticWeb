using System;
using System.Diagnostics;
using NullGuard;

namespace RomanticWeb.Ontologies
{
    /// <summary>
    /// Base class for RDF terms (properties and classes)
    /// </summary>
    [DebuggerDisplay("{TermName}")]
    public abstract class Term
    {
        private Ontology _ontology;

        /// <summary>
        /// Creates a new instance of names RDF term
        /// </summary>
        protected Term(string termName)
        {
            TermName = termName;
        }

        /// <summary>
        /// Gets the <see cref="Term"/>'s URI
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public Uri Uri
        {
            get
            {
                if (_ontology == null)
                {
                    throw new InvalidOperationException("Ontology isn't set");
                }

                return new Uri(Ontology.BaseUri + TermName);
            }
        }

        /// <summary>
        /// Gets the <see cref="Ontology"/>, which defines this <see cref="Term"/>
        /// </summary>
        public Ontology Ontology
        {
            get
            {
                return _ontology;
            }

            internal set
            {
                if (_ontology != null)
                {
                    throw new InvalidOperationException("Ontology is already set");
                }

                _ontology = value;
            }
        }

        internal string Prefix
        {
            get
            {
                return Ontology == null ? "?" : Ontology.Prefix;
            }
        }

        /// <summary>
        /// Gets the term name
        /// </summary>
        /// <remarks>Essentially it is a relative URI or hash part (depending on ontology namespace)</remarks>
        protected string TermName { get; private set; }

#pragma warning disable 1591
        public static bool operator ==([AllowNull] Term left, [AllowNull] Term right)
        {
            return Equals(left, right);
        }

        public static bool operator !=([AllowNull] Term left, [AllowNull] Term right)
        {
            return !Equals(left, right);
        }

        public override bool Equals([AllowNull] object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            if (obj.GetType() != this.GetType()) { return false; }
            return Equals((Term)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Uri != null ? Uri.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Ontology != null ? Ontology.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (TermName != null ? TermName.GetHashCode() : 0);
                return hashCode;
            }
        }

        protected bool Equals([AllowNull] Term other)
        {
            return Equals(Uri, other.Uri) && Equals(Ontology, other.Ontology) && string.Equals(TermName, other.TermName);
        }
#pragma warning restore 1591
    }
}