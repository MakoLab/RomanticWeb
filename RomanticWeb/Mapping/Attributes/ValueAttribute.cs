using System;

namespace RomanticWeb.Mapping.Attributes
{
    /// <summary>Maps the dictionary's value property to an RDF predicate.</summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class ValueAttribute:TermMappingAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueAttribute"/> class.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="term">The term.</param>
        public ValueAttribute(string prefix,string term)
            :base(prefix,term)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueAttribute"/> class.
        /// </summary>
        /// <param name="termUri">The term URI.</param>
        public ValueAttribute(string termUri)
            :base(termUri)
        {
        }
    }
}