using System;
using Anotar.NLog;

namespace RomanticWeb.Mapping.Attributes
{
    /// <summary>Base class for mapping attributes.</summary>
    public abstract class TermMappingAttribute:Attribute
    {
        #region Fields
        private readonly string _prefix;
        private readonly string _term;
        private readonly Uri _uri;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TermMappingAttribute"/> class.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="term">The term.</param>
        protected TermMappingAttribute(string prefix,string term)
        {
            _prefix=prefix;
            _term=term;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TermMappingAttribute"/> class.
        /// </summary>
        /// <param name="termUri">The term URI.</param>
        protected TermMappingAttribute(string termUri)
        {
            _uri=new Uri(termUri);
        }

        #endregion

        #region Properties
        /// <summary>Gets the ontology prefix.</summary>
        public string Prefix { get { return _prefix; } }

        /// <summary>Gets the term.</summary>
        public string Term { get { return _term; } }
        #endregion

        #region Non-public methods
        /// <summary>
        /// Gets the term URI.
        /// </summary>
        /// <exception cref="MappingException"></exception>
        protected Uri GetTermUri(MappingContext mappingContext)
        {
            Uri uri=_uri??mappingContext.OntologyProvider.ResolveUri(Prefix,Term);
            if (uri==null)
            {
                var message=string.Format("Cannot resolve QName {0}:{1}",Prefix,Term);
                LogTo.Fatal(message);
                throw new MappingException(message);
            }

            return uri;
        }
        #endregion
    }
}