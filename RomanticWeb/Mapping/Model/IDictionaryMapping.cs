using System;
using RomanticWeb.Converters;

namespace RomanticWeb.Mapping.Model
{
    /// <summary>
    /// Represents a dictionary property mapping
    /// </summary>
    public interface IDictionaryMapping : IPropertyMapping
    {
        /// <summary>
        /// Gets the key predicate mapping.
        /// </summary>
        Uri KeyPredicate { get; }

        /// <summary>
        /// Gets the value predicate mapping.
        /// </summary>
        Uri ValuePredicate { get; }

        INodeConverter KeyConverter { get; }

        INodeConverter ValueConverter { get; }
    }
}