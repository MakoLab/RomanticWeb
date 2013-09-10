using System;

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
    }
}