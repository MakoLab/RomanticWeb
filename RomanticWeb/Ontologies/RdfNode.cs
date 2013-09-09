using System;
using NullGuard;

namespace RomanticWeb.Ontologies
{
    /// <summary>
    /// Represents an RDF node (URI or literal)
    /// </summary>
    /// <remarks>Blank nodes are not supported currently</remarks>
    public sealed class RdfNode
    {
        private string _literal;
        private string _language;
        private Uri _dataType;
        private Uri _uri;

        private RdfNode() { }

        /// <summary>
        /// Gets the value indicating that the node is a URI
        /// </summary>
        public bool IsUri
        {
            get { return _uri != null; }
        }

        /// <summary>
        /// Gets the value indicating that the node is a literal
        /// </summary>
        public bool IsLiteral
        {
            get { return _literal != null; }
        }

        /// <summary>
        /// Gets the URI of a URI node
        /// </summary>
        /// <exception cref="InvalidOperationException">thrown when node is a literal</exception>
        public Uri Uri
        {
            get
            {
                if (IsLiteral)
                {
                    throw new InvalidOperationException("Literal node does not have a Uri");
                }

                return _uri;
            }
            private set { _uri = value; }
        }

        /// <summary>
        /// Gets the string value of a literal node
        /// </summary>
        /// <exception cref="InvalidOperationException">thrown when node is URI</exception>
        public string Literal
        {
            get
            {
                if (IsUri)
                {
                    throw new InvalidOperationException("Uri node does not have a literal value");
                }

                return _literal;
            }
            private set { _literal = value; }
        }

        /// <summary>
        /// Gets the data type of a literal node
        /// </summary>
        /// <exception cref="InvalidOperationException">thrown when node is URI</exception>
        [AllowNull]
        public Uri DataType
        {
            get
            {
                if (IsUri)
                {
                    throw new InvalidOperationException("Uri node does not have a data type");
                }

                return _dataType;
            }
            set { _dataType = value; }
        }

        /// <summary>
        /// Gets the language tag of a literal node
        /// </summary>
        /// <exception cref="InvalidOperationException">thrown when node is URI</exception>
        [AllowNull]
        public string Language
        {
            get
            {
                if (IsUri)
                {
                    throw new InvalidOperationException("Uri node does not have a language tag");
                }

                return _language;
            }
            set { _language = value; }
        }

        /// <summary>
        /// Gets the string representation of a node
        /// </summary>
        /// <returns>Literal value or URI for literal and URI nodes respectively</returns>
        public override string ToString()
        {
            if (IsLiteral)
            {
                return Literal;
            }
            if(IsUri)
            {
                return Uri.ToString();
            }

            throw new InvalidOperationException();
        }

        /// <summary>
        /// Factory method for creating URI nodes
        /// </summary>
        public static RdfNode ForUri(Uri uri)
        {
            return new RdfNode
                {
                    Uri = uri
                };
        }

        /// <summary>
        /// Factory method for creating literal nodes
        /// </summary>
        public static RdfNode ForLiteral(string value, [AllowNull] string language, [AllowNull] Uri dataType)
        {
            return new RdfNode
                {
                    Literal = value,
                    Language = language,
                    DataType = dataType
                };
        }
    }
}