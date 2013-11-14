using System;
using NullGuard;

namespace RomanticWeb.Linq.Model
{
    /// <summary>Expresses a prefix in the query.</summary>
    public class Prefix:QueryComponent
    {
        #region Fields
        private string _namespacePrefix;
        private Uri _namespaceUri;
        #endregion

        #region Constructors
        /// <summary>Default parameterles constructor.</summary>
        public Prefix():base()
        {
        }

        /// <summary>Constructs a complete prefix.</summary>
        /// <param name="namespacePrefix">Namespace prefix.</param>
        /// <param name="namespaceUri">Namespace URI.</param>
        public Prefix(string namespacePrefix,Uri namespaceUri)
        {
            _namespacePrefix=namespacePrefix;
            _namespaceUri=namespaceUri;
        }
        #endregion

        #region Properties
        /// <summary>Gets or sets a namespace prefix.</summary>
        [AllowNull]
        public string NamespacePrefix { get { return _namespacePrefix; } set { _namespacePrefix=value; } }

        /// <summary>Gets or sets a namespace URI.</summary>
        [AllowNull]
        public Uri NamespaceUri { get { return _namespaceUri; } set { _namespaceUri=value; } }
        #endregion

        #region Public methods
        /// <summary>Creates a string representation of this prefix.</summary>
        /// <returns>String representation of this prefix.</returns>
        public override string ToString()
        {
            return System.String.Format("PREFIX {0}: <{1}> ",(_namespacePrefix!=null?_namespacePrefix.ToString():System.String.Empty),(_namespaceUri!=null?_namespaceUri.ToString():System.String.Empty));
        }
        #endregion
    }
}