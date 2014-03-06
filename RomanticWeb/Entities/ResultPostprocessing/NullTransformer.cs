using NullGuard;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Entities.ResultPostprocessing
{
    [NullGuard(ValidationFlags.None)]
    public class NullTransformer : IResultTransformer
    {
        public object GetTransformed(IEntityProxy parent,IPropertyMapping property,IEntityContext context,object value)
        {
            return value;
        }

        public void SetTransformed(object value,IEntityStore store)
        {
            throw new System.NotImplementedException();
        }
    }
}