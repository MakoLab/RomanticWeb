using System;
using RomanticWeb.Entities;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Fluent;
using RomanticWeb.TestEntities;

namespace RomanticWeb.Tests.IntegrationTests
{
	public class DefaultGraphTypeMapping : EntityMap<ITypedEntity>
	{
        public DefaultGraphTypeMapping()
		{
            Class.Is("rdfs","Class").NamedGraph.SelectedBy<TestSelector>();
            Collection(p => p.Types).Term.Is("rdf", "type").NamedGraph.SelectedBy<TestSelector>().StoreAs.SimpleCollection();
		}

	    public class TestSelector:IGraphSelectionStrategy
	    {
	        public Uri SelectGraph(EntityId entityId)
	        {
	            return new Uri(entityId.Uri.AbsoluteUri.Replace("magi","data.magi"));
	        }
	    }
	}
}
