using System;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Mapping.Attributes
{
    /// <summary>Maps a property to an RDF predicate.</summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyAttribute:MappingAttribute
    {
        #region Fields
        private readonly string _propertyName;
        private Uri[] _range=new Uri[0];
        private int _cardinality=Int32.MaxValue;

        #endregion

        #region Constructors
        /// <summary>Default constructor with namespace prefix and predicate name passed.</summary>
        /// <param name="prefix">Namespace prefix.</param>
        /// <param name="propertyName">Predicate name.</param>
        public PropertyAttribute(string prefix,string propertyName):base(prefix)
        {
            _propertyName=propertyName;
        }

        #endregion

        #region Properties
        /// <summary>Gets a predicate name.</summary>
        public string PropertyName { get { return _propertyName; } }

        /// <summary>Gets or sets an array of Uri's beeing a range of given predicate.</summary>
        public Uri[] Range { get { return _range; } set { _range=(value??new Uri[0]); } }

        /// <summary>Gets or sets cardinality of the predicate.</summary>
        public int Cardinality { get { return _cardinality; } set { _cardinality = value; } }

        #endregion

        #region Internal methods
        internal IPropertyMapping GetMapping(Type propertyType,string propertyName,MappingContext mappingContext)
        {
            Uri uri=mappingContext.OntologyProvider.ResolveUri(Prefix,PropertyName);
            if (uri!=null)
            {
                return GetMappingInternal(propertyType, propertyName, uri, mappingContext);
            }

            throw new MappingException(string.Format("Cannot resolve property {0}:{1}", Prefix, PropertyName));
        }
        #endregion

        /// <summary>
        /// Creates a <see cref="PropertyMapping"/>
        /// </summary>
        protected virtual IPropertyMapping GetMappingInternal(Type propertyType,string propertyName,Uri uri,MappingContext mappingContext)
        {
            return new PropertyMapping(propertyType, propertyName, uri, mappingContext.DefaultGraphSelector);
        }
    }
}