using System.Collections.Generic;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Model;

namespace RomanticWeb.Converters
{
    /// <summary>
    /// Defines a contract for converting <see cref="Node"/>s to value objects or entities
    /// </summary>
    public interface INodeConverter
    {
        /// <summary>
        /// Converts <see cref="Node"/>s and checks for validity against destination property mapping
        /// </summary>
        /// <remarks>
        ///     <ul>
        ///         <li>Returns typed instances of <see cref="Entity"/> based on property's return value</li>
        ///         <li>
        ///             Doesn't check the type of literals against the property's return type (todo: implement this check)
        ///         </li>
        ///     </ul>
        /// </remarks>
        IEnumerable<object> ConvertNodes(IEnumerable<Node> objects,IPropertyMapping predicate);

        /// <summary>
        /// Converts <see cref="Node"/>s to most appropriate type based on raw RDF data
        /// </summary>
        /// <remarks>This will always return untyped instanes of <see cref="Entity"/> for URI nodes</remarks>
        IEnumerable<object> ConvertNodes(IEnumerable<Node> objects);

        IEnumerable<Node> ConvertBack(object value,IPropertyMapping property);
    }
}