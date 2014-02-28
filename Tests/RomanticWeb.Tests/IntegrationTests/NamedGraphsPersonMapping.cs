using System;
using RomanticWeb.Mapping.Fluent;
using RomanticWeb.TestEntities;

namespace RomanticWeb.Tests.IntegrationTests
{
    public class NamedGraphsPersonMapping : EntityMap<IPerson>
    {
        public NamedGraphsPersonMapping()
        {
            Property(p => p.FirstName)
                .Term.Is(new Uri("http://xmlns.com/foaf/0.1/givenName"));

            Property(p => p.LastName).Term.Is(new Uri("http://xmlns.com/foaf/0.1/familyName"));

            Property(p => p.Homepage).Term.Is(new Uri("http://xmlns.com/foaf/0.1/homePage"));

            Property(p => p.Friend).Term.Is("foaf","knows");

            Property(p => p.Friends).Term.Is("foaf", "friends");

            Class.Is("foaf","Person");

            Collection(p => p.Interests)
                .Term.Is("foaf", "interest")
                .StoreAs.RdfList();
        }
    }
}