using System;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Model;
using RomanticWeb.NamedGraphs;

namespace RomanticWeb.Tests.Stubs
{
    [Obsolete("Use configuration on ContextFactory when implemented")]
    public class TestGraphSelector : INamedGraphSelector
    {
        public Uri SelectGraph(EntityId entityId, IEntityMapping entityMapping, IPropertyMapping predicate)
        {
            EntityId nonBlankId = entityId;
            if (nonBlankId is BlankId)
            {
                nonBlankId = ((BlankId)nonBlankId).RootEntityId;
            }

            return new Uri(System.Text.RegularExpressions.Regex.Replace((nonBlankId != null ? nonBlankId.Uri.AbsoluteUri : ((BlankId)entityId).Graph.AbsoluteUri), "((?<!data.)magi)", "data.magi"));
        }
    }
}