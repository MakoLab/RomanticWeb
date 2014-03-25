using System.Reflection;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Fluent
{
    /// <summary>A mapping definition for properties.</summary>
    public sealed class PropertyMap:PropertyMapBase<PropertyMap>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMap"/> class.
        /// </summary>
        /// <param name="propertyInfo">The property.</param>
        public PropertyMap(PropertyInfo propertyInfo)
            :base(propertyInfo)
        {
        }

        /// <inheritdoc/>
        public override ITermPart<PropertyMap> Term
        {
            get
            {
                return new TermPart<PropertyMap>(this);
            }
        }

        /// <inheritdoc/>
        public override IPropertyMappingProvider Accept(IFluentMapsVisitor fluentMapsVisitor)
        {
            return fluentMapsVisitor.Visit(this);
        }
    }
}