using System;
using System.Collections.Generic;

namespace RomanticWeb.TestEntities
{
	public interface IPerson : IEntity
	{
		string FirstName { get; }
		string LastName { get; }
		Uri Homepage { get; }
		ICollection<string> Interests { get; }
	}
}