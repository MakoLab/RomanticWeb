using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Entities.ResultPostprocessing
{
    /// <summary>
    /// Contract from implementing transformation of a RDF objects
    /// </summary>
    public interface IResultTransformer
    {
        /// <summary>
        /// Gets the transformed result.
        /// </summary>
        /// <param name="parent">The parent entity.</param>
        /// <param name="property">The property.</param>
        /// <param name="context">The context.</param>
        /// <param name="value">The value.</param>
        object GetTransformed(IEntityProxy parent,IPropertyMapping property,IEntityContext context,object value);

        void SetTransformed(object value,IEntityStore store);
    }
}