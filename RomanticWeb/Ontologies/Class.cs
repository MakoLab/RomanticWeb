using System.Diagnostics.CodeAnalysis;

namespace RomanticWeb.Ontologies
{
    /// <summary>
    /// Represents an RDF class
    /// </summary>
    public class Class : Term
    {
        /// <summary>
        /// Creates a new instance of <see cref="Class"/>
        /// </summary>
        /// <param name="className"></param>
        public Class(string className)
            : base(className)
        {
        }

        /// <summary>
        /// Gets the class name
        /// </summary>
        /// <remarks>See remarks under <see cref="Term.TermName"/></remarks>
        public string ClassName { get { return TermName; } }

#pragma warning disable 1591
        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            string prefix = Ontology == null ? "_" : Ontology.Prefix;
            return string.Format("{0}:{1}", prefix, ClassName);
        }
#pragma warning restore
    }
}