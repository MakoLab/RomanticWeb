namespace RomanticWeb.Ontologies
{
    /// <summary>
    /// Base class for RDF terms (properties and classes)
    /// </summary>
    public abstract class RdfTerm
    {
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
    }
}