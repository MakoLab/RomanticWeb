using System;

namespace RomanticWeb.Entities
{
    /// <summary>
    /// Represents error, which occurs when the number of results is unexpected
    /// </summary>
    public class CardinalityException : Exception
    {
        private const string ErrorFormat = "Expected {0} objects but {1} found";
        private readonly int _expectedCardinality;
        private readonly int _actualCardinality;

        internal CardinalityException(int expectedCardinality, int actualCardinality)
        {
            _expectedCardinality = expectedCardinality;
            _actualCardinality = actualCardinality;
        }

        /// <summary>
        /// Expected number of results
        /// </summary>
        public int ExpectedCardinality
        {
            get
            {
                return _expectedCardinality;
            }
        }

        /// <summary>
        /// Actual number of results returned
        /// </summary>
        public int ActualCardinality
        {
            get
            {
                return _actualCardinality;
            }
        }

#pragma warning disable 1591
        public override string ToString()
        {
            return string.Format(ErrorFormat, ExpectedCardinality, ActualCardinality);
        }
#pragma warning restore
    }
}