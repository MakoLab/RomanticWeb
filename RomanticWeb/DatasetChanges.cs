using System.Collections.Generic;
using RomanticWeb.Model;

namespace RomanticWeb
{
    public sealed class DatasetChanges
    {
        internal DatasetChanges(IEnumerable<EntityTriple> addedTriples,IEnumerable<EntityTriple> removedTriples)
        {
            TriplesAdded=addedTriples;
            TriplesRemoved = removedTriples;
        }

        internal DatasetChanges()
        {
            TriplesAdded=new List<EntityTriple>();
            TriplesRemoved=new List<EntityTriple>();
        }

        public IEnumerable<EntityTriple> TriplesAdded { get; set; }

        public IEnumerable<EntityTriple> TriplesRemoved { get; set; }
    }
}