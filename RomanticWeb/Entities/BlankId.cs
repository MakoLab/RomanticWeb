using System;
using System.ComponentModel.Composition;
using System.Text.RegularExpressions;
using NullGuard;

namespace RomanticWeb
{
	[Export(typeof(EntityId))]
	public sealed class BlankId:EntityId
	{
		private static readonly Regex MatchingSchemeRegularExpression=new Regex("_[a-zA-Z]+:.+");

		/// <summary>MEF importing constructor.</summary>
		public BlankId(string id):this(id,null)
		{
		}

		public BlankId(string id,[AllowNull] Uri graphUri)
		{
			Id=id;
			GraphUri=graphUri;
		}

		[ImportingConstructor]
		protected BlankId():this("_:bnode00000")
		{
		}

		public string Id { get; private set; }

		public Uri GraphUri { get; private set; }

		/// <summary>Gets a regular expression matching blank node identifier.</summary>
		public override Regex MatchingScheme { get { return MatchingSchemeRegularExpression; } }
	}
}