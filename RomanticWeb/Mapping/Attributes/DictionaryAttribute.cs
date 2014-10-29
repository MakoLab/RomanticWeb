using System;
using System.Linq;
using System.Reflection;
using NullGuard;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Attributes
{
    /// <summary>Maps a dictionary and it's key/value properties to an RDF predicate.</summary>
    public sealed class DictionaryAttribute : PropertyAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryAttribute"/> class.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="term">The term.</param>
        public DictionaryAttribute(string prefix, string term)
            : base(prefix, term)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryAttribute"/> class.
        /// </summary>
        /// <param name="termUri">The term URI.</param>
        public DictionaryAttribute(string termUri)
            : base(termUri)
        {
        }

        /// <summary>Gets or sets the key converter type.</summary>
        public Type KeyConverterType { [return: AllowNull] get; set; }

        /// <summary>Gets or sets the value converter type.</summary>
        public Type ValueConverterType { [return: AllowNull] get; set; }

        internal override IPropertyMappingProvider Accept(IMappingAttributesVisitor visitor, PropertyInfo property)
        {
            var keyAttribute = property.GetCustomAttributes<KeyAttribute>().SingleOrDefault();
            var valueAttribute = property.GetCustomAttributes<ValueAttribute>().SingleOrDefault();
            return visitor.Visit(this, property, visitor.Visit(keyAttribute), visitor.Visit(valueAttribute));
        }
    }
}