using System;
using System.ComponentModel.Composition;
using System.Text.RegularExpressions;
using NullGuard;

namespace RomanticWeb
{
	/// <summary>
	/// Represents na Entity's identifies
	/// </summary>
	/// <remarks>Currently only URIs are supported</remarks>
	[Export(typeof(EntityId))]
	public class UriId:EntityId
	{
		private static readonly Regex MatchingSchemeRegularExpression=new Regex("[a-zA-Z][a-zA-Z0-9\\+\\.\\-]*:.+");
		private readonly Uri _uri;

		/// <summary>
		/// Creates a new instance of <see cref="EntityId"/> from string
		/// </summary>
		public UriId(string uri):this(new Uri(uri))
		{
		}

		/// <summary>
		/// Creates a new instance of <see cref="EntityId"/> from an Uniform Resource Identifies
		/// </summary>
		public UriId(Uri uri)
		{
			_uri=uri;
		}

		/// <summary>MEF importing constructor.</summary>
		[ImportingConstructor]
		protected UriId():this("urn:uuid:00000000-0000-0000-0000-000000000000")
		{
		}

		/// <summary>Gets a regular expression matching URI adress specification.</summary>
		public override Regex MatchingScheme { get { return MatchingSchemeRegularExpression; } }

		/// <summary>
		/// The underlying Uniform Resource Identifier
		/// </summary>
		public Uri Uri
		{
			get { return _uri; }
		}

		/// <summary>
		/// Checks if two Entity identifiers are equal
		/// </summary>
		public static bool operator==([AllowNull] UriId left,[AllowNull] UriId right)
		{
			return Equals(left,right);
		}

		/// <summary>
		/// Checks if two Entity identifiers are not equal
		/// </summary>
		public static bool operator!=([AllowNull] UriId left,[AllowNull] UriId right)
		{
			return !(left==right);
		}

		/// <summary>
		/// Gets the hash code
		/// </summary>
		public override int GetHashCode()
		{
			return _uri.GetHashCode();
		}

		/// <summary>
		/// Checks if two Entity identifiers are equal
		/// </summary>
		public override bool Equals([AllowNull] object obj)
		{
			if (obj==null||GetType()!=obj.GetType())
			{
				return false;
			}

			if (ReferenceEquals(this,obj))
			{
				return true;
			}

			return _uri==((UriId)obj)._uri;
		}

		public override string ToString()
		{
			return Uri.ToString();
		}

		/// <summary>
		/// Check for equality with <param name="other"></param>
		/// </summary>
		protected bool Equals([AllowNull] UriId other)
		{
			return other!=null&&Equals(this._uri,other._uri);
		}
	}
}