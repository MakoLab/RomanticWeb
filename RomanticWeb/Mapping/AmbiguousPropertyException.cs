namespace RomanticWeb.Mapping
{
    public class AmbiguousPropertyException:MappingException
    {
        internal AmbiguousPropertyException(string propertyName)
            :base(string.Format("Ambiguous property '{0}'", propertyName))
        {
        }
    }
}