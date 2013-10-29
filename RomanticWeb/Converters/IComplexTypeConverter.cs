using RomanticWeb.Entities;

namespace RomanticWeb.Converters
{
    /// <summary>
    /// Defines a contract for converts a complex RDF structure to an object
    /// </summary>
    public interface IComplexTypeConverter
    {
        /// <summary>
        /// Converts the node (and additional nodes) into an object
        /// </summary>
        /// <param name="objectNode">th root node of the structure</param>
        /// <param name="entityStore">store, from which relevant additional nodes are read</param>
        object Convert(IEntity objectNode, IEntityStore entityStore);

        /// <summary>
        /// Checks whether a node can be converted
        /// </summary>
        bool CanConvert(IEntity objectNode, IEntityStore entityStore);
    }
}