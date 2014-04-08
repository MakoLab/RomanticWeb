namespace RomanticWeb.Mapping
{
    /// <summary>
    /// Represents an error which occurs, when entity mapping contains
    /// multiple property mappings for a property
    /// </summary>
    public class AmbiguousPropertyException:MappingException
    {
        internal AmbiguousPropertyException(string propertyName)
            :base(string.Format("Ambiguous property '{0}'", propertyName))
        {
        }
    }
}