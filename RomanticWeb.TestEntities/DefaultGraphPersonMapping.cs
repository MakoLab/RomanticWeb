using System;
using RomanticWeb.Mapping.Fluent;

namespace RomanticWeb.TestEntities
{
	public class DefaultGraphPersonMapping : EntityMap<IPerson>
	{
		public DefaultGraphPersonMapping()
		{
			Property(p => p.FirstName).Predicate.Is("foaf", "givenName");
			Property(p => p.LastName).Predicate.Is(new Uri("http://xmlns.com/foaf/0.1/familyName"));
			Property(p => p.Homepage).Predicate.Is(new Uri("http://xmlns.com/foaf/0.1/homePage"));
            Collection(p => p.Interests).Predicate.Is(new Uri("http://xmlns.com/foaf/0.1/topic_interest"));
            Collection(p => p.NickNames).Predicate.Is(new Uri("http://xmlns.com/foaf/0.1/nick"));
            Collection(p => p.Friends).Predicate.Is(new Uri("http://xmlns.com/foaf/0.1/knows"));
		}
	}
}
