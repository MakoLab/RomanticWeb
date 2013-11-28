using System;
using System.Collections.Generic;
using System.Linq;
using RomanticWeb.Entities;
using RomanticWeb.Model;

namespace RomanticWeb
{
    /// <summary>
    /// Represents changes made the triple store
    /// </summary>
    public sealed class DatasetChanges
    {
        internal DatasetChanges(
            IEnumerable<EntityQuad> addedTriples, 
            IEnumerable<EntityQuad> removedTriples, 
            IEnumerable<Tuple<Uri, EntityId>> metaGraphChanges,
            IEnumerable<EntityId> deletedEntites)
        {
            QuadsAdded=addedTriples;
            QuadsRemoved = removedTriples;
            MetaGraphChanges=metaGraphChanges;
            DeletedEntites=deletedEntites;
        }

        internal DatasetChanges()
            :this(new EntityQuad[0],new EntityQuad[0],new Tuple<Uri,EntityId>[0],new EntityId[0])
        {
        }

        /// <summary>
        /// Gets the added quads
        /// </summary>
        public IEnumerable<EntityQuad> QuadsAdded { get; private set; }

        /// <summary>
        /// Gets the quads removed
        /// </summary>
        public IEnumerable<EntityQuad> QuadsRemoved { get; private set; }

        /// <summary>
        /// Gets the changes to meta graph
        /// </summary>
        public IEnumerable<Tuple<Uri, EntityId>> MetaGraphChanges { get; private set; }

        /// <summary>
        /// Gets the entities marked for deletion
        /// </summary>
        public IEnumerable<EntityId> DeletedEntites { get; private set; }

        /// <summary>
        /// Gets a value idicating whether there are any changes
        /// </summary>
        public bool Any 
        {
            get
            {
                return (QuadsAdded.Any())||(QuadsRemoved.Any())||(DeletedEntites.Any());
            }
        }
    }
}