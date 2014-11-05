using System;

namespace RomanticWeb.Mapping.Attributes
{
    /// <summary>Maps the dictionary's key property to an RDF predicate.</summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class KeyAttribute : TermMappingAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyAttribute"/> class.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="term">The term.</param>
        public KeyAttribute(string prefix, string term)
            : base(prefix, term)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyAttribute"/> class.
        /// </summary>
        /// <param name="termUri">The term URI.</param>
        public KeyAttribute(string termUri)
            : base(termUri)
        {
        }
    }
}