using RomanticWeb.Converters;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Attributes;

namespace RomanticWeb.Collections
{
    public interface IRdfListNode<TOwner,TConverter,T>:IEntity 
        where TConverter:INodeConverter
    {
        IRdfListNode<TOwner,TConverter,T> Rest { get; set; }

        T First { get; set; }
    }
}