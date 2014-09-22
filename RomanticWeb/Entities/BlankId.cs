using System;
using System.Diagnostics;
using NullGuard;
using RomanticWeb.Model;

namespace RomanticWeb.Entities
{
    /// <summary>
    /// A Blank Node identifier
    /// </summary>
    /// <remarks>Internally it is stored as a node:// URI, similarily to the Virtuoso way</remarks>
    [DebuggerDisplay("_:{_identifier,nq}")]
    [DebuggerTypeProxy(typeof(DebuggerViewProxy))]
    public sealed class BlankId : EntityId
    {
        private readonly string _identifier;
        private readonly Uri _graph;
        private readonly EntityId _root;

        /// <summary>Constructor for creating blank node idenifiers from scratch.</summary>
        /// <param name="identifier">Internal identifier.</param>
        /// <param name="root">Optional owning <see cref="IEntity" />'s identifier.</param>
        /// <param name="graphUri">Optional graph Uri.</param>
        public BlankId(string identifier, EntityId root = null, Uri graphUri = null)
            : this(CreateBlankNodeUri(identifier, graphUri), root)
        {
            _identifier = identifier;
            _graph = graphUri;
        }

        private BlankId(Uri blankNodeUri, EntityId root)
            : base(blankNodeUri)
        {
            while (root is BlankId)
            {
                root = ((BlankId)root).RootEntityId;
            }

            _root = root;
        }

        /// <summary>Gets the identifier of a root non-blank entity.</summary>
        [AllowNull]
        public EntityId RootEntityId { get { return _root; } }

        /// <summary>Gets the internal identifier of this blank node.</summary>
        public string Identifier { get { return _identifier; } }

        /// <summary>Gets the graph Uri of this blank node.</summary>
        [AllowNull]
        public Uri Graph { get { return _graph; } }

        public override string ToString()
        {
            return string.Format("_:{0}", _identifier);
        }

        internal static Uri CreateBlankNodeUri(string blankNodeId, Uri graphUri)
        {
            return new Uri(String.Format("node://{0}/{1}", blankNodeId, graphUri));
        }

        internal Node ToNode()
        {
            return Node.ForBlank(_identifier, RootEntityId, _graph);
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