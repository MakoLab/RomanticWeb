using System;
using System.Collections.Generic;
using RomanticWeb.Entities;

namespace RomanticWeb.TestEntities
{
    public interface IPerson : IEntity
    {
        string FirstName { get; set; }

        string LastName { get; set; }

        Uri Homepage { get; }

        IList<IEntity> HomepageAsEntities { get; }

        ICollection<string> Interests { get; set; }

        ICollection<string> NickNames { get; }

        IList<IPerson> Friends { get; set; }

        IList<string> FriendsAsLiterals { get; set; }

        IPerson Friend { get; }

        IEntity Entity { get; set; }

        int Age { get; set; }
    }
}