namespace RomanticWeb.Ontologies
{
    /// <summary>
    /// An Datatype property as defined in the OWL standard
    /// </summary>
    public sealed class DatatypeProperty : Property
    {
        /// <summary>
        /// Creates a new instance of <see cref="DatatypeProperty"/>
        /// </summary>
        public DatatypeProperty(string predicateName) : base(predicateName)
        {
        }
    }
}