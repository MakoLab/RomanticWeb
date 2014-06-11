namespace RomanticWeb.Collections
{
    internal interface IRdfListAdapter<T>
    {
        IRdfListNode<T> Head { get; }

        void Add(T item);
    }
}