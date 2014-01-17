using System;
using System.Diagnostics;
using RomanticWeb.Model;

namespace RomanticWeb.Entities
{
	/// <summary>
	/// A Blank Node identifier
	/// </summary>
    /// <remarks>Internally it is stored as a node:// URI, similarily to the Virtuoso way</remarks>
    [DebuggerDisplay("_:{_identifier,nq}")]
    [DebuggerTypeProxy(typeof(DebuggerViewProxy))]
	public sealed class BlankId:EntityId
	{
	    private readonly string _identifier;
	    private readonly Uri _graph;
	    private readonly EntityId _root;

        internal BlankId(string identifier,EntityId root,Uri graphUri=null)
            : this(CreateBlankNodeUri(identifier,graphUri),root)
        {
            _identifier=identifier;
            _graph=graphUri;
        }

	    private BlankId(Uri blankNodeUri, EntityId root)
            : base(blankNodeUri)
        {
	        while (root is BlankId)
	        {
	            root=((BlankId)root).RootEntityId;
	        }

            if (root==null)
            {
                throw new ArgumentException("No root EntityId found","root");
            }

            _root = root;
        }

        /// <summary>
        /// Gets the identifier of a root non-blank entity.
        /// </summary>
	    public EntityId RootEntityId
        {
	        get
	        {
	            return _root;
	        }
	    }

        internal static Uri CreateBlankNodeUri(string blankNodeId, Uri graphUri)
        {
            return new Uri(string.Format("node://{0}/{1}", blankNodeId, graphUri));
        }

        internal Node ToNode()
        {
            return Node.ForBlank(_identifier,RootEntityId,_graph);
        }

        private class DebuggerViewProxy
        {
            private readonly BlankId _entityId;

            public DebuggerViewProxy(BlankId entityId)
            {
                _entityId = entityId;
            }

            public string Identifier
            {
                get
                {
                    return _entityId._identifier;
                }
            }

            public EntityId RootEntityId
            {
                get
                {
                    return _entityId.RootEntityId;
                }
            }

            public Uri Graph
            {
                get
                {
                    return _entityId._graph;
                }
            }
        }
	}
}