using System;
using NullGuard;
using RomanticWeb.Entities;

namespace RomanticWeb.Model
{
    /// <summary>
    /// Represents a triple (subject, predicate, object)
    /// </summary>
    public sealed class EntityQuad : Triple,IComparable, IComparable<EntityQuad>
    {
        private readonly Node _graph;

        private readonly EntityId _entityId;

        /// <summary>
        /// Creates a new instance of <see cref="EntityQuad"/> in named graph
        /// </summary>
        public EntityQuad(EntityId entityId,Node s,Node p,Node o,[AllowNull]Node graph)
            :this(entityId,s,p,o)
        {
            if ((graph!=null)&&(!graph.IsUri))
            {
                throw new ArgumentOutOfRangeException("p", "Graph must be a URI");
            }

            _graph=graph;
        }

        /// <summary>
        /// Creates a new instance of <see cref="EntityQuad"/> in default graph
        /// </summary>
        public EntityQuad(EntityId entityId,Node s,Node p,Node o):base(s,p,o)
        {
            _entityId = entityId;
        }

        /// <summary>
        /// Gets the named graph node or null, if triple is in named graph
        /// </summary>
        [AllowNull]
        public Node Graph
        {
            get
            {
                return _graph;
            }
        }

        /// <summary>
        /// Gets entity id, which defines this triple
        /// </summary>
        public EntityId EntityId
        {
            get
            {
                return _entityId;
            }
        }

#pragma warning disable 1591
        public static bool operator ==(EntityQuad left, [AllowNull] EntityQuad right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EntityQuad left, [AllowNull] EntityQuad right)
        {
            return !Equals(left, right);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            return obj is EntityQuad && Equals((EntityQuad)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (_graph != null ? _graph.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ _entityId.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}",Subject,Predicate,Object,Graph);
        }

        int IComparable<EntityQuad>.CompareTo(EntityQuad other)
        {
            return ((IComparable)this).CompareTo(other);
        }

        int IComparable.CompareTo(object other)
        {
            return FluentCompare<EntityQuad>
                .Arguments(this, other)
                .By(t => t.EntityId)
                .By(t => t.Graph)
                .By(t => t.Subject)
                .By(t => t.Predicate)
                .By(t => t.Object)
                .End();
        }
#pragma warning restore

        internal EntityQuad InGraph([AllowNull]Uri graphUri)
        {
            if (graphUri != null)
            {
                return new EntityQuad(EntityId, Subject, Predicate, Object, Node.ForUri(graphUri));
            }

            return this;
        }

        private bool Equals(EntityQuad other)
        {
            return base.Equals(other) && Equals(_graph, other._graph) && _entityId.Equals(other._entityId);
        }
    }
}