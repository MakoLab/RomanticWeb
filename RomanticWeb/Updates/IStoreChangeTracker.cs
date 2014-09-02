using System;
using System.Collections.Generic;
using RomanticWeb.Entities;
using RomanticWeb.Model;

namespace RomanticWeb.Updates
{
    public interface IStoreChangeTracker
    {
        bool HasChanges { get; }

        void ReplacePredicateValues(EntityId id, Node predicate, Node[] newValues, Uri graph);

        void AssertEntity(EntityId entityId, IEnumerable<EntityQuad> quads);

        void Delete(EntityId entityId, DeleteBehaviour deleteBehaviour);
    }

    internal class DefaultStoreChangeTracker : IStoreChangeTracker
    {
        public bool HasChanges
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void ReplacePredicateValues(EntityId id, Node predicate, Node[] newValues, Uri graph)
        {
            throw new NotImplementedException();
        }

        public void AssertEntity(EntityId entityId, IEnumerable<EntityQuad> quads)
        {
            throw new NotImplementedException();
        }

        public void Delete(EntityId entityId, DeleteBehaviour deleteBehaviour)
        {
            throw new NotImplementedException();
        }
    }
}