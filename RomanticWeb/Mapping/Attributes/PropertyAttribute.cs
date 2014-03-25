using System;
using System.Reflection;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Attributes
{
    /// <summary>Maps a property to an RDF predicate.</summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=false)]
    public class PropertyAttribute:TermMappingAttribute
    {
        #region Constructors
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

        #endregion

        #region Public methods
        internal virtual IPropertyMappingProvider Accept(IMappingAttributesVisitor visitor,PropertyInfo property)
        {
            return visitor.Visit(this,property);
        }

        #endregion
    }
}