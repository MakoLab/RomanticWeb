using RomanticWeb.Entities;

namespace RomanticWeb.Updates
{
    public abstract class DatasetChange
    {
        private EntityId _changedEntity;

        protected DatasetChange(EntityId changedEntity)
        {
            _changedEntity = changedEntity;
        }
    }
}