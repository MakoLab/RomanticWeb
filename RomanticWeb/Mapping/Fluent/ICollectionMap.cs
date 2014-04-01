using RomanticWeb.Converters;

namespace RomanticWeb.Mapping.Fluent
{
    /// <summary>
    /// A mapping definition for collection properties
    /// </summary>
    public interface ICollectionMap
    {
        /// <summary>
        /// Gets a predicate map part
        /// </summary>
        ITermPart<ICollectionMap> Term { get; }

        /// <summary>
        /// Gets options for setting how this collection will be persisted
        /// </summary>
        StorageStrategyPart StoreAs { get; }

        ICollectionMap ConvertElementsWith<TConverter>()
            where TConverter:INodeConverter;
    }
}