using System;
using System.Reflection;
using NullGuard;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Fluent
{
    /// <summary>A base mapping definition for properties and collections.</summary>
    public abstract class PropertyMapBase : TermMap
    {
        private readonly PropertyInfo _propertyInfo;

        /// <summary>Initializes a new instance of the <see cref="PropertyMapBase"/> class.</summary>
        /// <param name="propertyInfo">The property.</param>
        protected PropertyMapBase(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
        }

        /// <summary>Gets the converter type.</summary>
        public virtual Type ConverterType { [return: AllowNull] get; internal set; }

        internal PropertyInfo PropertyInfo { get { return _propertyInfo; } }

        /// <summary>Accepts the specified fluent maps visitor.</summary>
        public abstract IPropertyMappingProvider Accept(IFluentMapsVisitor fluentMapsVisitor);
    }
}