namespace RomanticWeb.Mapping.Attributes
{
    public sealed class KeyAttribute:TermMappingAttribute
    {
        public KeyAttribute(string prefix,string term)
            :base(prefix,term)
        {
        }

        public KeyAttribute(string termUri)
            :base(termUri)
        {
        }
    }
}