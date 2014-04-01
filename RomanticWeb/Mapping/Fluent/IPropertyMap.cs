using RomanticWeb.Converters;

namespace RomanticWeb.Mapping.Fluent
{
    /// <summary>A mapping definition for properties.</summary>
    public interface IPropertyMap
    {
        /// <summary>
        /// Gets a predicate map part
        /// </summary>
        ITermPart<IPropertyMap> Term { get; }

        /// <Summary>
        /// Sets the converter type for this property
        /// </Summary>
        IPropertyMap ConvertWith<TConverter>() where TConverter : INodeConverter;
    }
}