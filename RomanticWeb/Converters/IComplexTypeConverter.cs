using System.Collections.Generic;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Model;

namespace RomanticWeb.Converters
{
    /// <summary>Defines a contract for converts a complex RDF structure to an object.</summary>
    public interface IComplexTypeConverter
    {
        /// <summary>Converts the node (and additional nodes) into an object.</summary>
        /// <param name="objectNode">The root node of the structure</param>
        /// <param name="entityStore">Store, from which relevant additional nodes are read</param>
        /// <param name="predicate">Predicate for this node.</param>
        object Convert(IEntity objectNode, IEntityStore entityStore, IPropertyMapping predicate);

        /// <summary>Checks whether a node can be converted.</summary>
        /// <param name="objectNode">Node to be checked.</param>
        /// <param name="entityStore">Entity store.</param>
        /// <param name="predicate">Predicate for this node.</param>
        bool CanConvert(IEntity objectNode, IEntityStore entityStore, IPropertyMapping predicate);

        /// <summary>Converts a value back to <see cref="Node"/>(s).</summary>
        /// <param name="obj">Object to be converted.</param>
        IEnumerable<Node> ConvertBack(object obj);

        /// <summary>Checks whether a value can be converted back to <see cref="Node"/>(s).</summary>
        /// <param name="value">Value to be checked.</param>
        /// <param name="predicate">Property mapping for this value.</param>
        bool CanConvertBack(object value,IPropertyMapping predicate);
    }
}