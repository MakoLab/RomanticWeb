using System.Diagnostics.CodeAnalysis;

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

        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            string prefix = Ontology == null ? "_" : Ontology.Prefix;
            return string.Format("Property {0}:{1}", prefix, PropertyName);
        }
    }
}