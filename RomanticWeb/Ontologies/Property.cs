namespace RomanticWeb.Ontologies
{
    /// <summary>
    /// A base classs for RDF properties
    /// </summary>
    public class Property : RdfTerm
    {
        /// <summary>
        /// Gets the term name
        /// </summary>
        /// <remarks>See remarks under <see cref="RdfTerm.TermName"/></remarks>
        public string PropertyName { get { return TermName; } }

        /// <summary>
        /// Creates a new Property
        /// </summary>
        internal Property(string predicateName)
            : base(predicateName)
        {
        }
    }
}