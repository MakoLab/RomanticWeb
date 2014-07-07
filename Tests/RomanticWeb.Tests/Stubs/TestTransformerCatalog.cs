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
            var collection = property as ICollectionMapping;
            if (collection != null)
            {
                switch (collection.StoreAs)
                {
                    case StoreAs.RdfList:
                        return new RdfListTransformer();
                    case StoreAs.SimpleCollection:
                        return new ObservableCollectionTransformer();
                }
            }

            return new SimpleTransformer(new OriginalResult());
        }
    }
}