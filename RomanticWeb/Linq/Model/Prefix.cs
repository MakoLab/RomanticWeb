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

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="operand">Type: <see cref="System.Object" />
        /// The object to compare with the current object.</param>
        /// <returns>Type: <see cref="System.Boolean" />
        /// <b>true</b> if the specified object is equal to the current object; otherwise, <b>false</b>.</returns>
        public override bool Equals(object operand)
        {
            return (!Object.Equals(operand,null))&&(operand.GetType()==typeof(Prefix))&&
                (_namespacePrefix!=null?_namespacePrefix.Equals(((Prefix)operand)._namespacePrefix):Object.Equals(((Prefix)operand)._namespacePrefix,null))&&
                (_namespaceUri!=null?_namespaceUri.Equals(((Prefix)operand)._namespaceUri):Object.Equals(((Prefix)operand)._namespaceUri,null));
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>Type: <see cref="System.Int32" />
        /// A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return typeof(Prefix).FullName.GetHashCode()^(_namespacePrefix!=null?_namespacePrefix.GetHashCode():0)^(_namespaceUri!=null?_namespaceUri.GetHashCode():0);
        }
        #endregion
    }
}