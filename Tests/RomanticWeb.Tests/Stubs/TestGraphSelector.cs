using System;
using RomanticWeb.Entities;
using RomanticWeb.Mapping;

namespace RomanticWeb.Tests.Stubs
{
    public class TestGraphSelector : GraphSelectionStrategyBase
    {
        protected override Uri GetGraphForEntityId(EntityId entityId)
        {
            return new Uri(entityId.Uri.AbsoluteUri.Replace("magi","data.magi"));
        }
    }
}