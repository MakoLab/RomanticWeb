using System;
using NullGuard;
using RomanticWeb.Entities;

namespace RomanticWeb
{
	/// <summary>
	/// Represents an RDF node (URI or literal)
	/// </summary>
	/// <remarks>Blank nodes are not supported currently</remarks>
	public sealed class Node
	{
		private readonly string _literal;
		private readonly string _language;
		private readonly Uri _dataType;
		private readonly Uri _uri;

		private Node(Uri uri)
		{
			_uri = uri;
		}

		private Node(string literal, string language, Uri dataType)
		{
			_literal = literal;
			_language = language;
			_dataType = dataType;
		}

		/// <summary>
		/// Gets the value indicating that the node is a URI
		/// </summary>
		public bool IsUri
		{
			get { return _uri != null&&_uri.Scheme!="node"; }
		}

		/// <summary>
		/// Gets the value indicating that the node is a literal
		/// </summary>
		public bool IsLiteral
		{
			get { return _literal != null; }
		}

	    /// <summary>
	    /// Gets the value indicating that the node is a blank node
	    /// </summary>
	    public bool IsBlank
	    {
	        get
	        {
                return _uri!=null&&_uri.Scheme == "node";
	        }
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
					throw new InvalidOperationException("Literal nodes do not have a Uri");
				}

				return _uri;
			}
		}

		/// <summary>
		/// Gets the string value of a literal node
		/// </summary>
		/// <exception cref="InvalidOperationException">thrown when node is URI</exception>
		public string Literal
		{
			get
			{
				if (IsUri || IsBlank)
				{
					throw new InvalidOperationException("Uri and blank nodes do not have a literal value");
				}

				return _literal;
			}
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
				if (IsUri || IsBlank)
				{
					throw new InvalidOperationException("Uri and blank nodes do not have a data type");
				}

				return _dataType;
			}
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
				if (IsUri || IsBlank)
				{
					throw new InvalidOperationException("Uri and blank nodes do not have a language tag");
				}

				return _language;
			}
		}

		/// <summary>
		/// Factory method for creating URI nodes
        /// </summary>
        /// todo:make extension method?
		public static Node ForUri(Uri uri)
		{
			return new Node(uri);
		}

		/// <summary>
		/// Factory method for creating literal nodes
        /// </summary>
        /// todo:make extension method?
		public static Node ForLiteral(string value, [AllowNull] string language, [AllowNull] Uri dataType)
		{
			return new Node(value, language, dataType);
		}

        /// <summary>
        /// Factory method for creating blank nodes
        /// </summary>
        /// todo:make extension method?
		public static Node ForBlank(string blankNodeId, [AllowNull] Uri graphUri)
		{
			return new Node(new Uri(string.Format("node://{0}/{1}",blankNodeId, graphUri)));
        }

#pragma warning disable 1591
        public static bool operator ==(Node left, Node right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Node left, Node right)
        {
            return !Equals(left, right);
        }
#pragma warning restore 1591

        public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) { return false; } 
			if (ReferenceEquals(this, obj)) { return true; }
			return obj is Node && Equals((Node) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode;

				if (IsLiteral)
				{
					hashCode=_literal.GetHashCode();
					hashCode=(hashCode*397)^(_language!=null?_language.GetHashCode():0);
					hashCode=(hashCode*397)^(_dataType!=null?_dataType.GetHashCode():0);
				}
				else
				{
					hashCode=_uri.AbsoluteUri.GetHashCode();
				}

				return hashCode;
			}
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

            if (IsUri||IsBlank)
            {
                return Uri.ToString();
            }

            throw new InvalidOperationException("Invalid node state");
        }

        /// <summary>
        /// Creates an <see cref="EntityId"/> for a <see cref="Node"/>
        /// </summary>
        /// todo:make extension method?
		public EntityId ToEntityId()
		{
            if (IsBlank)
            {
                return new BlankId(Uri);
            }

            if (IsUri)
            {
                return new EntityId(Uri);
            }

            throw new InvalidOperationException("Cannot convert literal node to EntityId");
		}

        private bool Equals(Node other)
        {
            if (IsLiteral)
            {
                return string.Equals(_literal, other._literal) && string.Equals(_language, other._language) && Equals(_dataType, other._dataType);
            }

            // is Uri
            return Equals(_uri.AbsoluteUri, other._uri.AbsoluteUri);
        }
	}
}