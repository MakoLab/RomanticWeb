using System;
using RomanticWeb.Entities;

namespace RomanticWeb.Updates
{
    /// <summary>
    /// Represents a change, which removes triples, which reference <see cref="RemoveReferences.Entity"/>
    /// </summary>
    public class RemoveReferences : DatasetChange
    {
        internal RemoveReferences(EntityId entity)
            : base(entity, null)
        {
            if (entity is BlankId)
            {
                throw new ArgumentOutOfRangeException("entity", "Reference cannot be a blank id");
            }
        }
    }
}