using VDS.RDF;

namespace RomanticWeb.Tests.Helpers
{
	public static class TripleStoreExtensions
	{
		 public static void LoadTestFile(this ITripleStore store, string fileName)
		 {
			 string resource = string.Format("RomanticWeb.Tests.TestGraphs.{0}, RomanticWeb.Tests", fileName);
			 store.LoadFromEmbeddedResource(resource);
		 }
	}
}