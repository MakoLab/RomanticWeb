using System;
using NullGuard;

namespace RomanticWeb
{
	public sealed class BlankId : EntityId
	{
		public BlankId(string id, [AllowNull] Uri graphUri)
		{
			Id = id;
			GraphUri = graphUri;
		}

		public string Id { get; private set; }

		public Uri GraphUri { get; private set; }
	}
}