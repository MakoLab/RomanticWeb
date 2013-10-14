using System;

namespace RomanticWeb.Entities
{
    public class CardinalityException:Exception
    {
        private const string ErrorFormat = "Expected {0} objects but {1} found";
        private readonly int _expectedCardinality;
        private readonly int _actualCardinality;

        public CardinalityException(int expectedCardinality,int actualCardinality)
        {
            _expectedCardinality=expectedCardinality;
            _actualCardinality=actualCardinality;
        }

        public int ExpectedCardinality
        {
            get
            {
                return _expectedCardinality;
            }
        }

        public int ActualCardinality
        {
            get
            {
                return _actualCardinality;
            }
        }

        public override string ToString()
        {
            return string.Format(ErrorFormat,ExpectedCardinality,ActualCardinality);
        }
    }
}