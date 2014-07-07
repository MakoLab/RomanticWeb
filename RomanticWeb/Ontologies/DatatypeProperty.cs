using System.Diagnostics.CodeAnalysis;

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
        public DatatypeProperty(string predicateName)
            : base(predicateName)
        {
        }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            string prefix = Ontology == null ? "_" : Ontology.Prefix;
            return string.Format("Datatype property {0}:{1}", prefix, PropertyName);
        }
    }
}