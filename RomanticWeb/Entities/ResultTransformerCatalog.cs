using System.Collections.Generic;
using RomanticWeb.Entities.ResultAggregations;
using RomanticWeb.Entities.ResultPostprocessing;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Entities
{
    /// <summary>
    /// Default implementation of <see cref="IResultTransformerCatalog"/>
    /// </summary>
    public sealed class ResultTransformerCatalog : IResultTransformerCatalog
    {
        private readonly IDictionary<Aggregation, IResultAggregator> _aggregations;
        private readonly IResultAggregator _fallbackAggregation = new OriginalResult();

        public ResultTransformerCatalog(IEnumerable<IResultAggregator> resultAggregators)
        {
            _aggregations = new Dictionary<Aggregation, IResultAggregator>();

            foreach (var resultAggregator in resultAggregators)
            {
                _aggregations[resultAggregator.Aggregation] = resultAggregator;
            }
        }

        /// <inheritdoc />
        public IResultAggregator GetAggregator(Aggregation aggregation)
        {
            if (_aggregations.ContainsKey(aggregation))
            {
                return _aggregations[aggregation];
            }

            return _fallbackAggregation;
        }

        /// <inheritdoc />
        public IResultTransformer GetTransformer(IPropertyMapping property)
        {
            var collectionMapping = property as ICollectionMapping;
            if (collectionMapping != null)
            {
                if (collectionMapping.StoreAs == StoreAs.RdfList)
                {
                    return new RdfListTransformer();
                }

                if (collectionMapping.StoreAs == StoreAs.SimpleCollection)
                {
                    return new ObservableCollectionTransformer();
                }
            }
            else if (property is IDictionaryMapping)
            {
                return new DictionaryTransformer();
            }

            return new SimpleTransformer(new SingleOrDefault());
        }
    }
}