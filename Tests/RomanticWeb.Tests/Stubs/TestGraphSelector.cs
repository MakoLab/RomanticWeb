using System;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Model;
using RomanticWeb.NamedGraphs;

namespace RomanticWeb.Tests.Stubs
{
    [Obsolete("Use configuration on ContextFactory when implemented")]
    public class TestGraphSelector:INamedGraphSelector
    {
        public Uri SelectGraph(EntityId entityId,IEntityMapping entityMapping,IPropertyMapping predicate)
        {
            var nonBlankId = entityId;
            while (nonBlankId is BlankId)
            {
                nonBlankId = ((BlankId)entityId).RootEntityId;

                if (nonBlankId == null)
                {
                    throw new ArgumentException("Blank node must have parent id", "entityId");
                }
            }

            return new Uri(nonBlankId.Uri.AbsoluteUri.Replace("magi", "data.magi"));
        }
    }
}