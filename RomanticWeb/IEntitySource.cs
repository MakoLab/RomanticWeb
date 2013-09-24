using System;
using System.Collections.Generic;
using RomanticWeb.Entities;
using RomanticWeb.Ontologies;

namespace RomanticWeb
{
    public interface IEntitySource
    {
        IEnumerable<Tuple<RdfNode,RdfNode,RdfNode>> GetNodesForQuery(string sparqlConstruct);

        void LoadEntity(IEntityStore store,EntityId entityId);
    }
}