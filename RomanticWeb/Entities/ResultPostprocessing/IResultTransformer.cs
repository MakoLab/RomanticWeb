using System.Collections.Generic;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Model;

namespace RomanticWeb.Entities.ResultPostprocessing
{
    /// <summary>
    /// Contract from implementing transformation of a RDF objects
    /// </summary>
    public interface IResultTransformer
    {
        /// <summary>
        /// Gets the transformed result.
        /// </summary>
        /// <param name="proxy">The parent entity proxy.</param>
        /// <param name="property">The property.</param>
        /// <param name="context">The context.</param>
        /// <param name="nodes">The nodes.</param>
        object FromNodes(IEntityProxy proxy, IPropertyMapping property, IEntityContext context, IEnumerable<Node> nodes);

        /// <summary>
        /// Gets the transformed result.
        /// </summary>
        /// <param name="proxy">The parent entity.</param>
        /// <param name="property">The property.</param>
        /// <param name="context">The context.</param>
        /// <param name="value">The object to transform.</param>
        IEnumerable<Node> ToNodes(object value, IEntityProxy proxy, IPropertyMapping property, IEntityContext context);
    }
}