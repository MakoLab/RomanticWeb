using System;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Mapping.Attributes
{
    /// <summary>Maps a property to an RDF predicate.</summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyAttribute:MappingAttribute
    {
        #region Fields
        private Uri[] _range=new Uri[0];
        private int _cardinality=Int32.MaxValue;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyAttribute"/> class.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="propertyName">Name of the property.</param>
        public PropertyAttribute(string prefix,string propertyName):base(prefix,propertyName)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyAttribute"/> class.
        /// </summary>
        /// <param name="propertyUri">The property URI.</param>
        public PropertyAttribute(string propertyUri):base(propertyUri)
        {
        }

        /// <summary>Gets or sets an array of Uri's beeing a range of given predicate.</summary>
        public Uri[] Range { get { return _range; } set { _range=(value??new Uri[0]); } }

        /// <summary>Gets or sets cardinality of the predicate.</summary>
        public int Cardinality { get { return _cardinality; } set { _cardinality = value; } }

        #endregion

        #region Non-public methods
        internal IPropertyMapping GetMapping(Type propertyType,string propertyName,MappingContext mappingContext)
        {
            return GetMappingInternal(propertyType, propertyName, GetTermUri(mappingContext), mappingContext);
        }

        /// <summary>Creates a <see cref="PropertyMapping"/>.</summary>
        protected virtual IPropertyMapping GetMappingInternal(Type propertyType,string propertyName,Uri uri,MappingContext mappingContext)
        {
            return new PropertyMapping(propertyType,propertyName,uri,mappingContext.DefaultGraphSelector);
        }
        #endregion
    }
}