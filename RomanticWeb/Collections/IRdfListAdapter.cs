namespace RomanticWeb.Collections
{
    internal interface IRdfListAdapter
    {
        IRdfListNode Head { get; }

        void Add(object item);
    }
}