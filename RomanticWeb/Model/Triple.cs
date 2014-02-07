using System;
using NullGuard;

namespace RomanticWeb.Model
{
    /// <summary>
    /// Reprents a triple, which does nto belong to a graph
    /// </summary>
    public class Triple : IComparable, IComparable<Triple>
    {
        private readonly Node _object;

        private readonly Node _subject;

        private readonly Node _predicate;

        /// <summary>
        /// Creates a new triple
        /// </summary>
        /// <param name="s">subject</param>
        /// <param name="p">predicate</param>
        /// <param name="o">object</param>
        public Triple(Node s,Node p,Node o)
        {
            if (!p.IsUri)
            {
                throw new ArgumentOutOfRangeException("p", "Predicate must be a URI");
            }

            if (s.IsLiteral)
            {
                throw new ArgumentOutOfRangeException("s", "Subject must be a URI or blank node");
            }

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

#pragma warning disable 1591    
        public static bool operator==([AllowNull] Triple left,[AllowNull] Triple right)
        {
            return Equals(left, right);
        }

        public static bool operator!=([AllowNull] Triple left,[AllowNull] Triple right)
        {
            return !Equals(left, right);
        }

        public override bool Equals([AllowNull] object obj)
        {
            if (ReferenceEquals(null,obj)) { return false; }
            if (ReferenceEquals(this,obj)) { return true; }
            var other=obj as Triple;
            return other!=null&&Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode=_object.GetHashCode();
                hashCode=(hashCode * 397)^_subject.GetHashCode();
                hashCode=(hashCode * 397)^_predicate.GetHashCode();
                return hashCode;
            }
        }

        int IComparable<Triple>.CompareTo(Triple other)
        {
            return ((IComparable)this).CompareTo(other);
        }

        int IComparable.CompareTo(object other)
        {
            return FluentCompare<Triple>
                .Arguments(this, other)
                .By(t => t.Subject)
                .By(t => t.Predicate)
                .By(t => t.Object)
                .End();
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", Subject, Predicate, Object);
        }

        protected bool Equals(Triple other)
        {
            return _object.Equals(other._object) && _subject.Equals(other._subject) && _predicate.Equals(other._predicate);
        }
#pragma warning restore
    }
}