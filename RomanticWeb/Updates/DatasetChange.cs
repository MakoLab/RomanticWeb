using RomanticWeb.Entities;

namespace RomanticWeb.Updates
{
    public abstract class DatasetChange
    {
        private readonly EntityId _changedEntity;
        private readonly EntityId _graph;

        protected DatasetChange(EntityId changedEntity, EntityId graph)
        {
            _changedEntity = changedEntity;
            _graph = graph;
        }

        public EntityId ChangedEntity
        {
            get
            {
                return _changedEntity;
            }
        }

        public EntityId Graph
        {
            get
            {
                return _graph;
            }
        }
    }
}