using System;
using System.Collections.Generic;
using System.Linq;

namespace RomanticWeb
{
    public class AmbiguousPropertyException : Exception
    {
        private readonly string _propertyName;
        private readonly IEnumerable<string> _matchingPrefixes;
        private const string MultipleMatchingPropertiesFormat = "Multiple properties found for {0}. Possible matches are: {1}";

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