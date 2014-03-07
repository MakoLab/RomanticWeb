using RomanticWeb.Entities.ResultAggregations;
using RomanticWeb.Entities.ResultPostprocessing;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Entities
{
    public interface IResultTransformerCatalog
    {
        IResultAggregator GetAggregator(Aggregation aggregation);

        IResultTransformer GetTransformer(IPropertyMapping property);
    }
}