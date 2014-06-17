using RomanticWeb.Entities;

namespace RomanticWeb.Collections
{
    /// <summary>
    /// Represents a rdf:List node, typed for a specific collection element of type <typeparamref name="T"/>
    /// </summary>
    public interface IRdfListNode<T>:IEntity
    {
        /// <summary>
        /// Gets or sets next list node
        /// </summary>
        IRdfListNode<T> Rest { get; set; }

        /// <summary>
        /// Gets or sets the current node's list element
        /// </summary>
        T First { get; set; }
    }
}