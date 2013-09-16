using VDS.RDF;

namespace RomanticWeb.DotNetRDF
{
	internal static class EntityIdExtensions
	{
		public static INode ToNode(this EntityId entityId, INodeFactory factory)
		{
			var blank = entityId as BlankId;
			if (blank != null)
			{
				return factory.CreateBlankNode(blank.Id);
			}

			return factory.CreateUriNode(((UriId)entityId).Uri);
		}
	}
}