using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Fluent
{
    /// <summary>
    /// A base mapping definition for properties and collections
    /// </summary>
    public abstract class PropertyMapBase<T>:PropertyMapBase
        where T:TermMap
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMapBase{T}"/> class.
        /// </summary>
        /// <param name="propertyInfo">The property.</param>
        protected PropertyMapBase(PropertyInfo propertyInfo)
            :base(propertyInfo)
        {
        }

        /// <summary>Gets the predicate map part.</summary>
        public abstract ITermPart<T> Term { get; }
    }

    /// <summary>
    /// A base mapping definition for properties and collections
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules","SA1402:FileMayOnlyContainASingleClass",Justification="Generic and non generic class in one file")]
    public abstract class PropertyMapBase:TermMap
    {
        private readonly PropertyInfo _propertyInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMapBase"/> class.
        /// </summary>
        /// <param name="propertyInfo">The property.</param>
        protected PropertyMapBase(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
        }

        internal PropertyInfo PropertyInfo
        {
            get
            {
                return _propertyInfo;
            }
        }

        /// <summary>
        /// Accepts the specified fluent maps visitor.
        /// </summary>
        public abstract IPropertyMappingProvider Accept(IFluentMapsVisitor fluentMapsVisitor);
    }
}