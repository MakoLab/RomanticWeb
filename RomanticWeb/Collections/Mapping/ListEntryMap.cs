using RomanticWeb.Converters;
using RomanticWeb.Mapping.Fluent;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Collections.Mapping
{
    /// <summary>A base class for mapping dynamically created type for dictionary entries.</summary>
    /// <typeparam name="TListNode">Type of entity, which contains the list.</typeparam>
    /// <typeparam name="T">The type of the dictionary value.</typeparam>
    /// <typeparam name="TConverter">converter type for list elements</typeparam>
    public abstract class ListEntryMap<TListNode, T, TConverter> : EntityMap<TListNode>
        where TListNode : IRdfListNode<T>
        where TConverter : INodeConverter
    {
        /// <summary>Initializes a new instance of the <see cref="ListEntryMap{TEntity,T,TConverter}"/> class.</summary>
        protected ListEntryMap()
        {
            Property(e => e.Rest).Term.Is(Rdf.rest).ConvertWith<AsEntityConverter<TListNode>>();
            Property(e => e.First).Term.Is(Rdf.first).ConvertWith<TConverter>();
        }
    }
}