using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Entities.ResultPostprocessing
{
    public interface IResultTransformer
    {
        object GetTransformed(IEntityProxy parent,IPropertyMapping property,IEntityContext context,object value);

        void SetTransformed(object value,IEntityStore store);
    }
}