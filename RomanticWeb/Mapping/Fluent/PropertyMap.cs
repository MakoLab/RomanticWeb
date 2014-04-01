using System.Reflection;
using RomanticWeb.Converters;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Fluent
{
    /// <summary>A mapping definition for properties.</summary>
    public sealed class PropertyMap:PropertyMapBase,IPropertyMap
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
        public ITermPart<IPropertyMap> Term
        {
            get
            {
                return new TermPart<PropertyMap>(this);
            }
        }

        /// <inheritdoc/>
        public IPropertyMap ConvertWith<TConverter>() where TConverter : INodeConverter
        {
            ConverterType = typeof(TConverter);
            return this;
        }

        /// <inheritdoc/>
        public override IPropertyMappingProvider Accept(IFluentMapsVisitor fluentMapsVisitor)
        {
            return fluentMapsVisitor.Visit(this);
        }
    }
}