using NullGuard;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Entities.ResultPostprocessing
{
    /// <summary>
    /// <see cref="IResultTransformer"/> which simply passes the original value
    /// </summary>
    [NullGuard(ValidationFlags.None)]
    public class NullTransformer:IResultTransformer
    {
        /// <summary>
        /// Does nothing
        /// </summary>
        /// <returns><paramref name="value"/></returns>
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