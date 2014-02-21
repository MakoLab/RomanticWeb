using System;

namespace RomanticWeb.Entities
{
    /// <summary>
    /// Contract for selecting base <see cref="Uri"/>s
    /// for relative <see cref="IEntity"/> identifiers
    /// </summary>
    public interface IBaseUriSelectionPolicy
    {
        /// <summary>
        /// Selects the base URI for the given entity identifier
        /// </summary>
        Uri SelectBaseUri(EntityId entityId);
    }
}