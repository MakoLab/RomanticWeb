using System;
using RomanticWeb.Converters;
using RomanticWeb.Mapping.Fluent;
using RomanticWeb.Model;
using RomanticWeb.TestEntities;

namespace RomanticWeb.Tests.IntegrationTests.TestMappings
{
    public class DefaultGraphPersonMapping : EntityMap<IPerson>
    {
        public DefaultGraphPersonMapping()
            : this(false)
        {
        }

        public DefaultGraphPersonMapping(bool useRdfLists)
        {
            Class.Is("foaf", "Person");
            Property(p => p.FirstName).Term.Is(new Uri("http://xmlns.com/foaf/0.1/givenName"));
            Property(p => p.LastName).Term.Is(new Uri("http://xmlns.com/foaf/0.1/familyName"));
            Collection(p => p.Homepage).Term.Is(new Uri("http://xmlns.com/foaf/0.1/homePage"));
            Collection(p => p.HomepageAsEntities).Term.Is(new Uri("http://xmlns.com/foaf/0.1/homePage"));
            Property(p => p.Friend).Term.Is(new Uri("http://xmlns.com/foaf/0.1/knows"));
            Property(p => p.Entity).Term.Is("dummy", "entity");
            Collection(p => p.Interests)
                .Term.Is(new Uri("http://xmlns.com/foaf/0.1/topic_interest"))
                .StoreAs.SimpleCollection();
            var nicknames = Collection(p => p.NickNames).Term.Is(new Uri("http://xmlns.com/foaf/0.1/nick"));
            var friends = Collection(p => p.Friends).Term.Is(new Uri("http://xmlns.com/foaf/0.1/knows"));
            var literals = Collection(p => p.FriendsAsLiterals).Term.Is(new Uri("http://xmlns.com/foaf/0.1/knows")).ConvertElementsWith<AsStringConverter>();

            if (useRdfLists)
            {
                friends.StoreAs.RdfList();
                literals.StoreAs.RdfList();
                nicknames.StoreAs.RdfList();
            }
            else
            {
                friends.StoreAs.SimpleCollection();
                literals.StoreAs.SimpleCollection();
                nicknames.StoreAs.SimpleCollection();
            }
        }
    }
}
