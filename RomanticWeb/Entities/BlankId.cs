using System;

namespace RomanticWeb.Entities
{
	/// <summary>
	/// A Blank Node indentifier
	/// </summary>
	/// <remarks>Internally it is stored as a node:// URI, similarily to the Virtuoso way</remarks>
	public sealed class BlankId:EntityId
	{
	    internal BlankId(Uri blankNodeUri)
            : base(blankNodeUri)
		{
		}
	}
}