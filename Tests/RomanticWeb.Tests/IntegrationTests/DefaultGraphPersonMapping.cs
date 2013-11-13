using System;
using RomanticWeb.Entities;
using RomanticWeb.Mapping;
using RomanticWeb.Mapping.Fluent;
using RomanticWeb.TestEntities;

namespace RomanticWeb.Tests.IntegrationTests
{
	public class DefaultGraphPersonMapping : EntityMap<IPerson>
	{
		public DefaultGraphPersonMapping()
		{
            Class.Is("foaf","Person").NamedGraph.SelectedBy<TestSelector>();
            Property(p => p.FirstName).Predicate.Is("foaf", "givenName").NamedGraph.SelectedBy<TestSelector>();
			Property(p => p.LastName).Predicate.Is(new Uri("http://xmlns.com/foaf/0.1/familyName")).NamedGraph.SelectedBy<TestSelector>();
            Property(p => p.Homepage).Predicate.Is(new Uri("http://xmlns.com/foaf/0.1/homePage")).NamedGraph.SelectedBy<TestSelector>();
            Property(p => p.Friend).Predicate.Is(new Uri("http://xmlns.com/foaf/0.1/knows")).NamedGraph.SelectedBy<TestSelector>();
            Property(p => p.Entity).Predicate.Is("dummy","entity").NamedGraph.SelectedBy<TestSelector>();
            Collection(p => p.Interests)
                .Predicate.Is(new Uri("http://xmlns.com/foaf/0.1/topic_interest"))
                .NamedGraph.SelectedBy<TestSelector>()
                .StoreAs.SimpleCollection();
            Collection(p => p.NickNames).Predicate.Is(new Uri("http://xmlns.com/foaf/0.1/nick")).NamedGraph.SelectedBy<TestSelector>();
            Collection(p => p.Friends).Predicate.Is(new Uri("http://xmlns.com/foaf/0.1/knows")).NamedGraph.SelectedBy<TestSelector>();
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
