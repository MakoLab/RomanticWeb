using System;

namespace RomanticWeb.Entities
{
    public class CardinalityException:Exception
    {
        private const string ErrorFormat = "Expected {0} objects but {1} found";
        private readonly int _expectedCount;
        private readonly int _actualCount;

        public CardinalityException(int expectedCount,int actualCount)
        {
            _expectedCount=expectedCount;
            _actualCount=actualCount;
        }

        public override string ToString()
        {
            return string.Format(ErrorFormat,_expectedCount,_actualCount);
        }
    }
}