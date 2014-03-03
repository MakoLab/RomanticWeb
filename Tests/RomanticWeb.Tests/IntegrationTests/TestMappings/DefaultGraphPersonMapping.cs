using System;
using RomanticWeb.Mapping.Fluent;
using RomanticWeb.TestEntities;

namespace RomanticWeb.Tests.IntegrationTests.TestMappings
{
	public class DefaultGraphPersonMapping : EntityMap<IPerson>
	{
	    public DefaultGraphPersonMapping():this(false)
	    {
	    }

        public DefaultGraphPersonMapping(bool useRdfLists)
        {
            Class.Is("foaf","Person");
            Property(p => p.FirstName).Term.Is(new Uri("http://xmlns.com/foaf/0.1/givenName"));
            Property(p => p.LastName).Term.Is(new Uri("http://xmlns.com/foaf/0.1/familyName"));
            Property(p => p.Homepage).Term.Is(new Uri("http://xmlns.com/foaf/0.1/homePage"));
            Property(p => p.Friend).Term.Is(new Uri("http://xmlns.com/foaf/0.1/knows"));
            Property(p => p.Entity).Term.Is("dummy","entity");
            Collection(p => p.Interests)
                .Term.Is(new Uri("http://xmlns.com/foaf/0.1/topic_interest"))
                .StoreAs.SimpleCollection();
            var nicknames=Collection(p => p.NickNames).Term.Is(new Uri("http://xmlns.com/foaf/0.1/nick"));
            var friends=Collection(p => p.Friends).Term.Is(new Uri("http://xmlns.com/foaf/0.1/knows"));

            if (useRdfLists)
            {
                friends.StoreAs.RdfList();
                nicknames.StoreAs.RdfList();
            }
		}
	}
}
