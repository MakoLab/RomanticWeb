using System;
using RomanticWeb.Mapping.Fluent;
using RomanticWeb.TestEntities;
using RomanticWeb.Tests.Stubs;

namespace RomanticWeb.Tests.IntegrationTests
{
	public class DefaultGraphPersonMapping : EntityMap<IPerson>
	{
		public DefaultGraphPersonMapping()
		{
            Class.Is("foaf", "Person").NamedGraph.SelectedBy<TestGraphSelector>();
            Property(p => p.FirstName).Term.Is(new Uri("http://xmlns.com/foaf/0.1/givenName")).NamedGraph.SelectedBy<TestGraphSelector>();
            Property(p => p.LastName).Term.Is(new Uri("http://xmlns.com/foaf/0.1/familyName")).NamedGraph.SelectedBy<TestGraphSelector>();
            Property(p => p.Homepage).Term.Is(new Uri("http://xmlns.com/foaf/0.1/homePage")).NamedGraph.SelectedBy<TestGraphSelector>();
            Property(p => p.Friend).Term.Is(new Uri("http://xmlns.com/foaf/0.1/knows")).NamedGraph.SelectedBy<TestGraphSelector>();
            Property(p => p.Entity).Term.Is("dummy", "entity").NamedGraph.SelectedBy<TestGraphSelector>();
            Collection(p => p.Interests)
                .Term.Is(new Uri("http://xmlns.com/foaf/0.1/topic_interest"))
                .NamedGraph.SelectedBy<TestGraphSelector>()
                .StoreAs.SimpleCollection();
            Collection(p => p.NickNames).Term.Is(new Uri("http://xmlns.com/foaf/0.1/nick")).NamedGraph.SelectedBy<TestGraphSelector>();
            Collection(p => p.Friends).Term.Is(new Uri("http://xmlns.com/foaf/0.1/knows")).NamedGraph.SelectedBy<TestGraphSelector>();
		}
	}
}
