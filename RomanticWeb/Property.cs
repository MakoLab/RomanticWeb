namespace RomanticWeb
{
    public class Property
    {
        public string PredicateUri { get; private set; }

        public Property(string predicateUri)
        {
            PredicateUri = predicateUri;
        }
    }
}