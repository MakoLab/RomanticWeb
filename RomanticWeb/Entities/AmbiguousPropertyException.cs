using System;
using System.Collections.Generic;
using System.Linq;

namespace RomanticWeb.Entities
{
    /// <summary>
    /// Represents an error, which occur when multiple properties are found for a predicate
    /// </summary>
	public class AmbiguousPropertyException : Exception
    {
        private const string MultipleMatchingPropertiesFormat = "Multiple properties found for {0}. Possible matches are: {1}";
		private readonly string _propertyName;
		private readonly IEnumerable<string> _matchingPrefixes;

		internal AmbiguousPropertyException(string propertyName, IEnumerable<string> matchingPrefixes)
		{
			_propertyName = propertyName;
			_matchingPrefixes = matchingPrefixes;
		}

		public override string Message
		{
			get
			{
				var qNames = _matchingPrefixes.Select(p => string.Format("{0}:{1}", p, _propertyName));
				return string.Format(MultipleMatchingPropertiesFormat, _propertyName, string.Join(", ", qNames));
			}
		}
	}
}