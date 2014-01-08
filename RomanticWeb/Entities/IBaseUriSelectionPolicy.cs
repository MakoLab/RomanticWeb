using System;

namespace RomanticWeb.Entities
{
    public interface IBaseUriSelectionPolicy
    {
        Uri SelectBaseUri(EntityId entityId);
    }
}