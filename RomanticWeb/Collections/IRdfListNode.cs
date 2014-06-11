using RomanticWeb.Entities;

namespace RomanticWeb.Collections
{
    public interface IRdfListNode<T>:IEntity
    {
        IRdfListNode<T> Rest { get; set; }

        T First { get; set; }
    }
}