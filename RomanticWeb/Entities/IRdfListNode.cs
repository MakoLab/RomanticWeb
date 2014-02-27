using RomanticWeb.Mapping.Attributes;

namespace RomanticWeb.Entities
{
    public interface IRdfListNode:IEntity
    {
        [Property("rdf","rest")]
        IRdfListNode Rest { get; set; }

        [Property("rdf","first")]
        object First { get; set; }
    }
}