using System;

namespace RomanticWeb
{
    /// <summary>
    /// Represents a triple (subject, predicate, object)
    /// </summary>
    public sealed class Triple
    {
        private readonly Node _object;

        private readonly Node _subject;

        private readonly Node _predicate;

        /// <summary>
        /// Creates a new instance of <see cref="Triple"/>
        /// </summary>
        /// <param name="s"></param>
        /// <param name="p"></param>
        /// <param name="o"></param>
        public Triple(Node s,Node p,Node o)
        {
            if (!p.IsUri)
            {
                throw new ArgumentOutOfRangeException("p","Predicate must be a URI");
            }
            
            if (s.IsLiteral)
            {
                throw new ArgumentOutOfRangeException("s", "Subject must be a URI or blank node");
            }

            _object=o;
            _subject=s;
            _predicate=p;
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
        public static bool operator ==(Triple left, Triple right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Triple left, Triple right)
        {
            return !Equals(left, right);
        }
#pragma warning restore 1591

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            if (obj.GetType() != typeof(Triple)) { return false; }

            return Equals((Triple)obj);
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

        private bool Equals(Triple other)
        {
            return _object.Equals(other._object) && _subject.Equals(other._subject) && _predicate.Equals(other._predicate);
        }
    }
}