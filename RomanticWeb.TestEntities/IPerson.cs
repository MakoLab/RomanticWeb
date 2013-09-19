using System;
using System.Collections.Generic;
using RomanticWeb.Entities;

namespace RomanticWeb.TestEntities
{
	public interface IPerson : IEntity
	{
		string FirstName { get; }
		string LastName { get; }

		Uri Homepage { get; }

		ICollection<string> Interests { get; }

	    IList<IPerson> Friends { get; }
	}
}