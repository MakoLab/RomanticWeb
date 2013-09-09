namespace RomanticWeb.Ontologies
{
    /// <summary>
    /// A base classs for RDF properties
    /// </summary>
    public abstract class Property : RdfTerm
    {
        /// <summary>
        /// Gets the term name
        /// </summary>
        /// <remarks>See remarks under <see cref="RdfTerm.TermName"/></remarks>
        public string PredicateName { get { return TermName; } }

        /// <summary>
        /// Creates a new Property
        /// </summary>
        protected Property(string predicateName)
            : base(predicateName)
        {
        }
    }
}