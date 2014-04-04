using System.Diagnostics.CodeAnalysis;

namespace RomanticWeb.Ontologies
{
	/// <summary>
	/// A base classs for RDF properties
	/// </summary>
	public class Property : Term
	{
		/// <summary>
		/// Creates a new Property
		/// </summary>
		internal Property(string predicateName)
			: base(predicateName)
		{
		}

        /// <summary>
        /// Gets the term name
        /// </summary>
        /// <remarks>See remarks under <see cref="Term.TermName"/></remarks>
        public string PropertyName { get { return TermName; } }

#pragma warning disable 1591
        [ExcludeFromCodeCoverage]
		public override string ToString()
		{
			string prefix = Ontology == null ? "_" : Ontology.Prefix;
			return string.Format("{0}:{1}", prefix, PropertyName);
        }
#pragma warning restore
	}
}