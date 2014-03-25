using RomanticWeb.Entities.ResultAggregations;
using RomanticWeb.Entities.ResultPostprocessing;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Entities
{
    /// <summary>
    /// A container for <see cref="IResultAggregator"/>s and <see cref="IResultTransformer"/>s
    /// </summary>
    public interface IResultTransformerCatalog
    {
        /// <summary>
        /// Gets an aggregator for <paramref name="aggregation"/>.
        /// </summary>
        /// <param name="aggregation">The aggregation.</param>
        IResultAggregator GetAggregator(Aggregation aggregation);

        /// <summary>
        /// Gets a transformer for transforming <paramref name="property"/>'s values.
        /// </summary>
        /// <param name="property">The property.</param>
        IResultTransformer GetTransformer(IPropertyMapping property);
    }
}