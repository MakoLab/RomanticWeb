using System;
using System.Collections.Generic;
using System.Linq;
using RomanticWeb.Entities;
using RomanticWeb.Model;

namespace RomanticWeb
{
    public sealed class DatasetChanges
    {
        internal DatasetChanges(
            IEnumerable<EntityQuad> addedTriples, 
            IEnumerable<EntityQuad> removedTriples, 
            IEnumerable<Tuple<Uri, EntityId>> metaGraphChanges,
            IEnumerable<EntityId> deletedEntites)
        {
            TriplesAdded=addedTriples;
            TriplesRemoved = removedTriples;
            MetaGraphChanges=metaGraphChanges;
            DeletedEntites=deletedEntites;
        }

        internal DatasetChanges()
            :this(new EntityQuad[0],new EntityQuad[0],new Tuple<Uri,EntityId>[0],new EntityId[0])
        {
        }

        public IEnumerable<EntityQuad> TriplesAdded { get; private set; }

        public IEnumerable<EntityQuad> TriplesRemoved { get; private set; }

        public IEnumerable<Tuple<Uri, EntityId>> MetaGraphChanges { get; private set; }

        public IEnumerable<EntityId> DeletedEntites { get; private set; }

        public bool Any 
        {
            get
            {
                return (TriplesAdded.Any())||(TriplesRemoved.Any())||(DeletedEntites.Any());
            }
        }
    }
}