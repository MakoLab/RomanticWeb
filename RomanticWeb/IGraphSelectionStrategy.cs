using System;

namespace RomanticWeb
{
    public interface IGraphSelectionStrategy
    {
        Uri SelectGraph(EntityId entityId);
    }
}