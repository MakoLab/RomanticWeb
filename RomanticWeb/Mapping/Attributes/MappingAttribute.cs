using System;

namespace RomanticWeb.Mapping.Attributes
{
    /// <summary>Base class for mapping attributes.</summary>
    public abstract class MappingAttribute:Attribute
    {
        #region Fields
        private readonly string _prefix;
        private readonly string _term;
        private readonly Uri _uri;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingAttribute"/> class.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="term">The term.</param>
        protected MappingAttribute(string prefix,string term)
        {
            _prefix=prefix;
            _term=term;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingAttribute"/> class.
        /// </summary>
        /// <param name="termUri">The term URI.</param>
        protected MappingAttribute(string termUri)
        {
            _uri=new Uri(termUri);
        }

        #endregion

        #region Properties
        /// <summary>Gets the ontology prefix.</summary>
        public string Prefix { get { return _prefix; } }

        /// <summary>
        /// Gets the term.
        /// </summary>
        protected string Term
        {
            get
            {
                return _term;
            }
        }

        #endregion

        /// <summary>
        /// Gets the term URI.
        /// </summary>
        /// <exception cref="MappingException"></exception>
        protected Uri GetTermUri(MappingContext mappingContext)
        {
            Uri uri = _uri??mappingContext.OntologyProvider.ResolveUri(Prefix,Term);
            if (uri==null)
            {
                throw new MappingException(string.Format("Cannot resolve QName {0}:{1}",Prefix,Term));
            }

            return uri;
        }
    }
}