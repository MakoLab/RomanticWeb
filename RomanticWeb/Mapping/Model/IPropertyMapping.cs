using System;
using RomanticWeb.Converters;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Model
{
    /// <summary>
    /// Mapping for an Entity's property
    /// </summary>
    public interface IPropertyMapping
    {
        /// <summary>
        /// Gets the entity mapping.
        /// </summary>
        IEntityMapping EntityMapping { get; }

        /// <summary>
        /// Gets the RDF predicate URI
        /// </summary>
        Uri Uri { get; }

        /// <summary>
        /// Gets the property name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the property's return type
        /// </summary>
        Type ReturnType { get; }

        /// <summary>
        /// Gets the property's declaring type
        /// </summary>
        Type DeclaringType { get; }

        /// <summary>
        /// Gets the converter.
        /// </summary>
        INodeConverter Converter { get; }

        /// <summary>
        /// Accepts the specified mapping model visitor.
        /// </summary>
        /// <param name="mappingModelVisitor">The mapping model visitor.</param>
        void Accept(IMappingModelVisitor mappingModelVisitor);
    }
}