using RomanticWeb.Entities;
using VDS.RDF;

namespace RomanticWeb.DotNetRDF
{
    internal static class EntityIdExtensions
    {
        public static INode ToNode(this EntityId entityId, INodeFactory factory)
        {
            if (entityId is BlankId)
            {
                return factory.CreateBlankNode(entityId.Uri.Authority);
            }

            return factory.CreateUriNode(entityId.Uri);
        }
    }
}