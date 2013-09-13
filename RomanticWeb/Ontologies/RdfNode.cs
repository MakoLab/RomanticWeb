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
		private string _blankNodeId;
		private Uri _graphUri;

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
		/// Gets the value indicating that the node is a blank node
		/// </summary>
		public bool IsBlank
		{
			get { return _blankNodeId != null; }
		}

		/// <summary>
		/// Gets the URI of a URI node
		/// </summary>
		/// <exception cref="InvalidOperationException">thrown when node is a literal</exception>
		public Uri Uri
		{
			get
			{
				if (IsLiteral || IsBlank)
				{
					throw new InvalidOperationException("Literal and blank nodes do not have a Uri");
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
				if (IsUri || IsBlank)
				{
					throw new InvalidOperationException("Uri and blank nodes do not have a literal value");
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
				if (IsUri || IsBlank)
				{
					throw new InvalidOperationException("Uri and blank nodes do not have a data type");
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
				if (IsUri || IsBlank)
				{
					throw new InvalidOperationException("Uri and blank nodes do not have a language tag");
				}

				return _language;
			}
			set { _language = value; }
		}

		[AllowNull]
		public Uri GraphUri
		{
			get
			{
				if (!IsBlank)
				{
					throw new InvalidOperationException("Graph URI is currently only used with blank nodes");
				}
				
				return _graphUri;
			}
			private set { _graphUri = value; }
		}

		public string BlankNodeId
		{
			get
			{
				if (!IsBlank)
				{
					throw new InvalidOperationException("Only blank nodes have blank node identifiers");
				} 
				
				return _blankNodeId;
			}
			private set { _blankNodeId = value; }
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
			if (IsUri)
			{
				return Uri.ToString();
			}
			if (IsBlank)
			{
				var graphString = GraphUri == null ? "default" : string.Format("<{0}>", GraphUri);
				return string.Format("_:{0} from graph {1}", BlankNodeId, graphString);
			}

			throw new InvalidOperationException("Invalid node state");
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

		public static RdfNode ForBlank(string blankNodeId, [AllowNull] Uri graphUri)
		{
			return new RdfNode
				{
					BlankNodeId = blankNodeId,
					GraphUri = graphUri
				};
		}

		public override int GetHashCode()
		{
			return (IsUri?(_uri!=null?_uri.AbsoluteUri.GetHashCode():base.GetHashCode()):(IsBlank?(_blankNodeId!=null?_blankNodeId.GetHashCode():base.GetHashCode()):(_literal!=null?_literal.GetHashCode():base.GetHashCode())));
		}
	}
}