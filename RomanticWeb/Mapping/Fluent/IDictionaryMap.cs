using RomanticWeb.Converters;

namespace RomanticWeb.Mapping.Fluent
{
    /// <summary>
    /// A mapping definition for dictionary properties
    /// </summary>
    public interface IDictionaryMap
    {
        /// <summary>Gets the predicate map part.</summary>
        ITermPart<IDictionaryMap> Term { get; }

        /// <summary>
        /// Gets the dictionary key predicate map part
        /// </summary>
        ITermPart<IDictionaryMap> KeyPredicate { get; }

        /// <summary>
        /// Gets the dictionary value predicate map part
        /// </summary>
        ITermPart<IDictionaryMap> ValuePredicate { get; }

        IDictionaryMap ConvertKeysWith<TConverter>() where TConverter : INodeConverter;

        IDictionaryMap ConvertValuesWith<TConverter>() where TConverter : INodeConverter;
    }
}