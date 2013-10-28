using System;
using System.Diagnostics;
using NullGuard;
using RomanticWeb.Entities;

namespace RomanticWeb.Model
{
    /// <summary>
    /// Represents a triple (subject, predicate, object)
    /// </summary>
    public sealed class EntityTriple : IComparable, IComparable<EntityTriple>
    {
        private readonly Node _object;

        private readonly Node _subject;

        private readonly Node _predicate;

        private readonly Node _graph;

        private readonly EntityId _entityId;

        /// <summary>
        /// Creates a new instance of <see cref="EntityTriple"/> in named graph
        /// </summary>
        public EntityTriple(EntityId entityId,Node s,Node p,Node o,Node graph)
            :this(entityId,s,p,o)
        {
            if (!graph.IsUri)
            {
                throw new ArgumentOutOfRangeException("p", "Graph must be a URI");
            }

            _graph=graph;
        }

        /// <summary>
        /// Creates a new instance of <see cref="EntityTriple"/> in default graph
        /// </summary>
        public EntityTriple(EntityId entityId,Node s,Node p,Node o)
        {
            if (!p.IsUri)
            {
                throw new ArgumentOutOfRangeException("p", "Predicate must be a URI");
            }

            if (s.IsLiteral)
            {
                throw new ArgumentOutOfRangeException("s", "Subject must be a URI or blank node");
            }

            _entityId = entityId;
            _subject = s;
            _predicate = p;
            _object = o;
        }

        /// <summary>
        /// Gets the triple's object
        /// </summary>
        public Node Object
        {
            get
            {
                return _object;
            }
        }

        /// <summary>
        /// Gets the triple's predicate
        /// </summary>
        public Node Predicate
        {
            get
            {
                return _predicate;
            }
        }

        /// <summary>
        /// Gets the triple's subject
        /// </summary>
        public Node Subject
        {
            get
            {
                return _subject;
            }
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
        public static bool operator ==(EntityTriple left, [AllowNull] EntityTriple right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EntityTriple left, [AllowNull] EntityTriple right)
        {
            return !Equals(left, right);
        }
#pragma warning restore 1591

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            if (obj.GetType() != typeof(EntityTriple)) { return false; }

            return Equals((EntityTriple)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = _object.GetHashCode();
                hashCode = (hashCode * 397) ^ _subject.GetHashCode();
                hashCode = (hashCode * 397) ^ _predicate.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}",Subject,Predicate,Object,Graph);
        }

        int IComparable<EntityTriple>.CompareTo(EntityTriple other)
        {
            return ((IComparable)this).CompareTo(other);
        }

        int IComparable.CompareTo(object other)
        {
            return FluentCompare<EntityTriple>
                .Arguments(this, other)
                .By(t => t.EntityId)
                .By(t => t.Graph)
                .By(t => t.Subject)
                .By(t => t.Predicate)
                .By(t => t.Object)
                .End();

            ////int result = EntityId.CompareTo(other);

            ////if (result==0)
            ////{
            ////    result=Graph.CompareTo(other.Graph);

            ////    if (result==0)
            ////    {
            ////        result=Subject.CompareTo(other.Subject);

            ////        if (result==0)
            ////        {
            ////            result=Predicate.CompareTo(other.Predicate);

            ////            if (result==0)
            ////            {
            ////                return Object.CompareTo(other.Object);
            ////            }
            ////        }
            ////    }
            ////}

            ////return result;
        }

        internal EntityTriple InGraph([AllowNull]Uri graphUri)
        {
            if (graphUri != null)
            {
                return new EntityTriple(EntityId, Subject, Predicate, Object, Node.ForUri(graphUri));
            }

            return this;
        }

        private bool Equals(EntityTriple other)
        {
            return _object.Equals(other._object) && _subject.Equals(other._subject) && _predicate.Equals(other._predicate) && Equals(_graph,other.Graph)&&_entityId.Equals(other.EntityId);
        }
    }
}