using RomanticWeb.Model;

namespace RomanticWeb.Converters
{
    /// <summary>
    /// Defines the contract for converting RDF nodes
    /// </summary>
    public interface INodeConverter
    {
        /// <summary>Converts a node to it's .NET representation</summary>
        object Convert(Node objectNode, IEntityContext context);

        /// <summary>Converts an object to an RDF node</summary>
        Node ConvertBack(object value);
    }
}