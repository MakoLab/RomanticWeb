using System;
using RomanticWeb.Entities;

namespace RomanticWeb.Updates
{
    public class EntityDelete : DatasetChange
    {
        public EntityDelete(EntityId entity)
            : base(entity)
        {
        }

        public override bool CanMergeWith(DatasetChange other)
        {
            return false;
        }

        public override DatasetChange MergeWith(DatasetChange other)
        {
            throw new InvalidOperationException("Cannot merge EntityDelete with any other change");
        }
    }
}