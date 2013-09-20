using System;

namespace RomanticWeb.Entities
{
	public sealed class BlankId:EntityId
	{
	    internal BlankId(string hashCode)
            : base(CreateHashedUri(hashCode))
		{
		}

	    private static Uri CreateHashedUri(string hashCode)
	    {
	        return new Uri("node://"+hashCode);
	    }
	}
}