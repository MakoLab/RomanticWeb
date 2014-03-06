using RomanticWeb.Entities;
using RomanticWeb.Entities.ResultAggregations;
using RomanticWeb.Entities.ResultPostprocessing;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Tests.Stubs
{
    public class TestTransformerCatalog : IResultTransformerCatalog
    {
        public IResultAggregator GetAggregator(Aggregation aggregation)
        {
            if (aggregation == Aggregation.Original)
            {
                return new OriginalResult();
            }

            return new SingleOrDefault();
        }

        public IResultTransformer GetTransformer(IPropertyMapping property)
        {
            switch (property.StorageStrategy)
            {
                case StorageStrategyOption.RdfList:
                    return new RdfListTransformer();
                case StorageStrategyOption.Simple:
                    return new ObservableCollectionTransformer();
                default:
                    return new NullTransformer();
            }
        }
    }
}