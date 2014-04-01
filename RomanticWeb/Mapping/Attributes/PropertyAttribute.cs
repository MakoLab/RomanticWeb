using System;
using System.Reflection;
using NullGuard;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Attributes
{
    /// <summary>Maps a property to an RDF predicate.</summary>
    [AttributeUsage(AttributeTargets.Property,AllowMultiple=false,Inherited=false)]
    public class PropertyAttribute:TermMappingAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyAttribute"/> class.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="propertyName">Name of the property.</param>
        public PropertyAttribute(string prefix,string propertyName):base(prefix,propertyName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyAttribute"/> class.
        /// </summary>
        /// <param name="propertyUri">The property URI.</param>
        public PropertyAttribute(string propertyUri):base(propertyUri)
        {
        }

        public Type ConverterType { [return:AllowNull]get; set; }

        internal virtual IPropertyMappingProvider Accept(IMappingAttributesVisitor visitor,PropertyInfo property)
        {
            return visitor.Visit(this,property);
        }
    }
}