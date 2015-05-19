using System.Globalization;
using RomanticWeb.Model;

namespace RomanticWeb.Converters
{
    /// <summary>Defines the contract for converting RDF nodes.</summary>
    public interface INodeConverter
    {
        /// <summary>Converts a node to it's .NET representation</summary>
        /// <param name="objectNode">Graph node to be converter.</param>
        /// <param name="context">Owning entity context.</param>
        object Convert(Node objectNode, IEntityContext context);

        /// <summary>Converts an object to an RDF node</summary>
        /// <param name="value">Value to be converted.</param>
        /// <param name="context">Owning entity context.</param>
        Node ConvertBack(object value, IEntityContext context);
    }
}