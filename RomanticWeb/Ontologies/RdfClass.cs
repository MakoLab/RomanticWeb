namespace RomanticWeb.Ontologies
{
    /// <summary>
    /// Represents an RDF class
    /// </summary>
    public class RdfClass : RdfTerm
    {
        /// <summary>
        /// Creates a new instance of <see cref="RdfClass"/>
        /// </summary>
        /// <param name="className"></param>
        public RdfClass(string className)
            : base(className)
        {
        }

        /// <summary>
        /// Gets the class name
        /// </summary>
        /// <remarks>See remarks under <see cref="RdfTerm.TermName"/></remarks>
        public string ClassName { get { return TermName; } }
    }
}