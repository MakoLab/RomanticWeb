using System;
using System.Linq;
using RomanticWeb.Converters;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Fluent;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Collections.Mapping
{
    /// <summary>A base class for mapping dynamically created type for dictionary entries.</summary>
    /// <typeparam name="TOwner">Owning type of this list map.</typeparam>
    /// <typeparam name="TConverter">Type of the value converter.</typeparam>
    /// <typeparam name="TEntity">Type of entity, which contains the list.</typeparam>
    /// <typeparam name="T">The type of the dictionary value.</typeparam>
    public sealed class ListEntryMap<TOwner,TConverter,TEntity,T>:EntityMap<TEntity>
        where TEntity:IRdfListNode<TOwner,TConverter,T>
        where TConverter:INodeConverter
    {
        /// <summary>Initializes a new instance of the <see cref="ListEntryMap{TOwner,TConverter,TEntity,T}"/> class.</summary>
        internal ListEntryMap(Type elementConverter)
        {
            Property(e => e.Rest).Term.Is(Rdf.rest).ConvertWith<AsEntityConverter<IRdfListNode<TOwner,TConverter,T>>>();
            IPropertyMap propertyMap=Property(e => e.First).Term.Is(Rdf.first);
            if (elementConverter!=null)
            {
                propertyMap.GetType()
                    .GetInterfaceMap(typeof(IPropertyMap)).InterfaceMethods
                    .Where(item => item.Name=="ConvertWith")
                    .Select(item => item.MakeGenericMethod(elementConverter))
                    .First().Invoke(propertyMap,null);
            }
        }
    }
}