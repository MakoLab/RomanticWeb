namespace RomanticWeb.Mapping.Attributes
{
    public sealed class ValueAttribute:TermMappingAttribute
    {
        public ValueAttribute(string prefix,string term)
            :base(prefix,term)
        {
        }

        public ValueAttribute(string termUri)
            :base(termUri)
        {
        }
    }
}