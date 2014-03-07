using System;
using System.Collections.Generic;
using RomanticWeb.ComponentModel.Composition;
using RomanticWeb.Entities.ResultAggregations;
using RomanticWeb.Entities.ResultPostprocessing;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Entities
{
    public sealed class ResultTransformerCatalog : IResultTransformerCatalog
    {
        private static readonly Lazy<IDictionary<Aggregation,IResultAggregator>> Aggregations;
        private readonly IResultAggregator _fallbackAggregation=new OriginalResult();

        static ResultTransformerCatalog()
        {
            Aggregations = new Lazy<IDictionary<Aggregation, IResultAggregator>>(delegate
            {
                var resultAggregations = new Dictionary<Aggregation,IResultAggregator>();
                foreach (var resultProcessingStrategy in ContainerFactory.GetInstancesImplementing<IResultAggregator>())
                {
                    resultAggregations[resultProcessingStrategy.Aggregation] = resultProcessingStrategy;
                }

                return resultAggregations;
            });
        }

        public IResultAggregator GetAggregator(Aggregation aggregation)
        {
            if (Aggregations.Value.ContainsKey(aggregation))
            {
                return Aggregations.Value[aggregation];
            }

            return _fallbackAggregation;
        }

        public IResultTransformer GetTransformer(IPropertyMapping property)
        {
            if (property.StorageStrategy==StorageStrategyOption.RdfList)
            {
                return new RdfListTransformer();
            }

            if (property.StorageStrategy==StorageStrategyOption.Simple)
            {
                return new ObservableCollectionTransformer();
            }

            return new NullTransformer();
        }
    }
}