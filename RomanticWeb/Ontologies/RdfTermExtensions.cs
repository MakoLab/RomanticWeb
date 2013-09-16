namespace RomanticWeb.Ontologies
{
	internal static class RdfTermExtensions
	{
		internal static T InOntology<T>(this T term, Ontology ontology) where T : RdfTerm
		{
			term.Ontology = ontology;
			return term;
		}
	}
}