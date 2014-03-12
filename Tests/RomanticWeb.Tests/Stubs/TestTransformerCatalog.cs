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
            var collection=property as ICollectionMapping;
            if (collection!=null)
            {
                switch (collection.StorageStrategy)
                {
                    case StorageStrategyOption.RdfList:
                        return new RdfListTransformer();
                    case StorageStrategyOption.Simple:
                        return new ObservableCollectionTransformer();
                }
            }

            return new NullTransformer();
        }
    }
}