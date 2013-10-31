using System;
using RomanticWeb.Mapping.Fluent;

namespace RomanticWeb.TestEntities
{
    public class NamedGraphsPersonMapping : EntityMap<IPerson>
    {
        public NamedGraphsPersonMapping()
        {
            // todo: add helper method to facilitate protocol replacement operation
            this.Property(p => p.FirstName)
                .Predicate.Is(new Uri("http://xmlns.com/foaf/0.1/givenName"))
                .NamedGraph.SelectedBy(id => new Uri(id.ToString().Replace("http", "personal")));

            this.Property(p => p.LastName).Predicate.Is(new Uri("http://xmlns.com/foaf/0.1/familyName"))
                                     .NamedGraph.SelectedBy(id => new Uri(id.ToString().Replace("http", "personal")));

            this.Property(p => p.Homepage).Predicate.Is(new Uri("http://xmlns.com/foaf/0.1/homePage"))
                                     .NamedGraph.SelectedBy(id => new Uri(id.ToString().Replace("http", "interestsOf")));

            this.Property(p => p.Friend).Predicate.Is("foaf","knows");
        }
    }
}