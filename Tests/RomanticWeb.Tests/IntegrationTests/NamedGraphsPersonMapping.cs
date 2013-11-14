using System;
using RomanticWeb.Mapping.Fluent;
using RomanticWeb.TestEntities;

namespace RomanticWeb.Tests.IntegrationTests
{
    public class NamedGraphsPersonMapping : EntityMap<IPerson>
    {
        public NamedGraphsPersonMapping()
        {
            // todo: add helper method to facilitate protocol replacement operation
            this.Property(p => p.FirstName)
                .Term.Is(new Uri("http://xmlns.com/foaf/0.1/givenName"))
                .NamedGraph.SelectedBy(id => new Uri(id.ToString().Replace("http", "personal")));

            this.Property(p => p.LastName).Term.Is(new Uri("http://xmlns.com/foaf/0.1/familyName"))
                                     .NamedGraph.SelectedBy(id => new Uri(id.ToString().Replace("http", "personal")));

            this.Property(p => p.Homepage).Term.Is(new Uri("http://xmlns.com/foaf/0.1/homePage"))
                                     .NamedGraph.SelectedBy(id => new Uri(id.ToString().Replace("http", "interestsOf")));

            this.Property(p => p.Friend).Term.Is("foaf", "knows")
                                     .NamedGraph.SelectedBy(id => new Uri(id.ToString().Replace("http","friendsOf")));

            this.Property(p => p.Friends).Term.Is("foaf", "friends");

            this.Class.Is("foaf","Person");

            Collection(p => p.Interests)
                .Term.Is("foaf", "interest")
                .NamedGraph.SelectedBy(id => new Uri(id.ToString().Replace("http", "interestsOf")))
                .StoreAs.RdfList();
        }
    }
}