using System;
using NullGuard;
using RomanticWeb.Entities;

namespace RomanticWeb.Mapping.Model
{
    internal class NullGraphSelector:IGraphSelectionStrategy
    {
        [return:AllowNull]
        public Uri SelectGraph(EntityId entityId)
        {
            return null;
        }
    }
}