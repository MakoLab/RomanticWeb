using System;
using NullGuard;

namespace RomanticWeb.Mapping.Attributes
{
    public abstract class PredicateMappingAttribute : TermMappingAttribute
    {
        protected PredicateMappingAttribute(string prefix, string term)
            : base(prefix, term)
        {
        }

        protected PredicateMappingAttribute(string termUri)
            : base(termUri)
        {
        }

        /// <summary>Gets or sets the type of the converter.</summary>
        public virtual Type ConverterType { [return: AllowNull] get; set; }
    }
}