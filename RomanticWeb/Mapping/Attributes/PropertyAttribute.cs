using System;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Mapping.Attributes
{
    /// <summary>Maps a property to an RDF predicate.</summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyAttribute:MappingAttribute
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

        #region Non-public methods
        internal IPropertyMapping GetMapping(Type propertyType,string propertyName,MappingContext mappingContext)
        {
            return GetMappingInternal(propertyType, propertyName, GetTermUri(mappingContext), mappingContext);
        }

        /// <summary>Creates a <see cref="PropertyMapping"/>.</summary>
        protected virtual IPropertyMapping GetMappingInternal(Type propertyType,string propertyName,Uri uri,MappingContext mappingContext)
        {
            return new PropertyMapping(propertyType, propertyName, uri);
        }

        #endregion
    }
}