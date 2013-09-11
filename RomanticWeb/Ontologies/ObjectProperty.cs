using System.Diagnostics.CodeAnalysis;

namespace RomanticWeb.Ontologies
{
    /// <summary>
    /// An object property as defined by the OWL standard
    /// </summary>
    public sealed class ObjectProperty : Property
    {
        /// <summary>
        /// Creates a new <see cref="ObjectProperty"/>
        /// </summary>
        public ObjectProperty(string predicateName) : base(predicateName)
        {
        }

        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            string prefix = Ontology == null ? "_" : Ontology.Prefix;
            return string.Format("Object property {0}:{1}", prefix, PropertyName);
        }
    }
}