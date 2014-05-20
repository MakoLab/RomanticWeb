using RomanticWeb.Converters;
using RomanticWeb.Entities;

namespace RomanticWeb.Collections
{
    internal interface IRdfListAdapter<TOwner,TConverter,T>
        where TConverter:INodeConverter
    {
        IRdfListNode<TOwner,TConverter,T> Head { get; }

        void Add(T item);
    }
}