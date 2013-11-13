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
            IEnumerable<Tuple<Uri,EntityId>> metaGraphChanges)
        {
            TriplesAdded=addedTriples;
            TriplesRemoved = removedTriples;
            MetaGraphChanges=metaGraphChanges;
        }

        internal DatasetChanges()
        {
            TriplesAdded=new List<EntityQuad>();
            TriplesRemoved=new List<EntityQuad>();
        }

        public IEnumerable<EntityQuad> TriplesAdded { get; private set; }

        public IEnumerable<EntityQuad> TriplesRemoved { get; private set; }

        public IEnumerable<Tuple<Uri, EntityId>> MetaGraphChanges { get; private set; }

        public bool Any 
        {
            get
            {
                return (TriplesAdded.Any())||(TriplesRemoved.Any());
            }
        }
    }
}