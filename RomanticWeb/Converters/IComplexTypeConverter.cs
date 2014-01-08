using System.Collections.Generic;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Model;

namespace RomanticWeb.Converters
{
    /// <summary>
    /// Defines a contract for converting complex RDF structures to objects
    /// </summary>
    public interface IComplexTypeConverter
    {
        /// <summary>Converts the node (and additional nodes) into an object.</summary>
        /// <param name="objectNode">The root node of the structure</param>
        /// <param name="entityStore">Store, from which relevant additional nodes are read</param>
        object Convert(IEntity objectNode,IEntityStore entityStore);

        /// <summary>Checks whether a node can be converted.</summary>
        bool CanConvert(IEntity objectNode,IEntityStore entityStore,IPropertyMapping predicate);

        /// <summary>Converts a value back to <see cref="Node"/>(s).</summary>
        IEnumerable<Node> ConvertBack(object obj);

        /// <summary>Checks whether a value can be converted back to <see cref="Node"/>(s).</summary>
        bool CanConvertBack(object value,IPropertyMapping predicate);
    }
}