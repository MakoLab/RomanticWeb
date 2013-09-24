using System;

namespace RomanticWeb.Entities
{
	public sealed class BlankId:EntityId
	{
	    internal BlankId(Uri blankNodeUri)
            : base(blankNodeUri)
		{
		}
	}
}