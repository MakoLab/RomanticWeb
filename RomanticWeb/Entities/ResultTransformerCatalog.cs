using System;
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
        private static readonly Lazy<IDictionary<Aggregation, IResultAggregator>> Aggregations;
        private readonly IResultAggregator _fallbackAggregation = new OriginalResult();

        static ResultTransformerCatalog()
        {
            Aggregations = new Lazy<IDictionary<Aggregation, IResultAggregator>>(delegate
            {
                var resultAggregations = new Dictionary<Aggregation, IResultAggregator>();
                ////foreach (var resultProcessingStrategy in ContainerFactory.GetInstancesImplementing<IResultAggregator>())
                ////{
                ////    resultAggregations[resultProcessingStrategy.Aggregation] = resultProcessingStrategy;
                ////}

                return resultAggregations;
            });
        }

        /// <inheritdoc />
        public IResultAggregator GetAggregator(Aggregation aggregation)
        {
            if (Aggregations.Value.ContainsKey(aggregation))
            {
                return Aggregations.Value[aggregation];
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