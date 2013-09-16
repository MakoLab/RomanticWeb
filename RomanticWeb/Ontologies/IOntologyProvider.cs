using System.Collections.Generic;

namespace RomanticWeb.Ontologies
{
	/// <summary>
	/// Defines methods for accessing metadata about ontologies
	/// </summary>
	public interface IOntologyProvider
	{
		/// <summary>
		/// Get ontologies' metadata
		/// </summary>
		IEnumerable<Ontology> Ontologies { get; }
	}
}