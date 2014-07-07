using System;
using RomanticWeb.Converters;

namespace RomanticWeb.Mapping.Fluent
{
    /// <summary>A mapping definition for collection properties.</summary>
    public interface ICollectionMap
    {
        /// <summary>Gets a predicate map part.</summary>
        ITermPart<ICollectionMap> Term { get; }

        /// <summary>Gets or sets an element converter type.</summary>
        /// <remarks>This type is used only for RDF lists.</remarks>
        Type ElementConverterType { get; }

        /// <summary>Gets options for setting how this collection will be persisted.</summary>
        StorageStrategyPart StoreAs { get; }

        /// <summary>Sets the converter type for the collection's elements.</summary>
        ICollectionMap ConvertElementsWith<TConverter>() where TConverter : INodeConverter;
    }
}