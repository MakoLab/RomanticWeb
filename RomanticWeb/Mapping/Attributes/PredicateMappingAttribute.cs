using System;
using NullGuard;

namespace RomanticWeb.Mapping.Attributes
{
    /// <summary>
    /// Base class for mapping an RDF predicate
    /// </summary>
    public abstract class PredicateMappingAttribute : TermMappingAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PredicateMappingAttribute"/> class.
        /// </summary>
        /// <param name="prefix">The term prefix.</param>
        /// <param name="term">The term name.</param>
        protected PredicateMappingAttribute(string prefix, string term)
            : base(prefix, term)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PredicateMappingAttribute"/> class.
        /// </summary>
        /// <param name="termUri">The term URI.</param>
        protected PredicateMappingAttribute(string termUri)
            : base(termUri)
        {
        }

        /// <summary>Gets or sets the type of the converter.</summary>
        public virtual Type ConverterType { [return: AllowNull] get; set; }
    }
}