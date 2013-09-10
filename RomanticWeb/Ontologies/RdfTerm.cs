using System;
using NullGuard;

namespace RomanticWeb.Ontologies
{
    /// <summary>
    /// Base class for RDF terms (properties and classes)
    /// </summary>
    public abstract class RdfTerm
    {
        private Uri _uri;
        private Ontology _ontology;

        /// <summary>
        /// Gets the term name
        /// </summary>
        /// <remarks>Essentially it is a relative URI or hash part (depending on ontology namespace)</remarks>
        protected string TermName { get; private set; }

        /// <summary>
        /// Creates a new instance of names RDF term
        /// </summary>
        protected RdfTerm(string termName)
        {
            TermName = termName;
        }

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

        public Ontology Ontology
        {
            get { return _ontology; }
            internal set
            {
                if (_ontology != null)
                {
                    throw new InvalidOperationException("Ontology is already set");
                }

                _ontology = value;
            }
        }

        public override bool Equals([AllowNull] object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RdfTerm) obj);
        }

        protected bool Equals([AllowNull] RdfTerm other)
        {
            return Equals(_uri, other._uri) && Equals(_ontology, other._ontology) && string.Equals(TermName, other.TermName);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (_uri != null ? _uri.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_ontology != null ? _ontology.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (TermName != null ? TermName.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==([AllowNull] RdfTerm left, [AllowNull] RdfTerm right)
        {
            return Equals(left, right);
        }

        public static bool operator !=([AllowNull] RdfTerm left, [AllowNull] RdfTerm right)
        {
            return !Equals(left, right);
        }
    }
}