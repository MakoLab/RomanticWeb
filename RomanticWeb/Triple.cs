using RomanticWeb.Ontologies;

namespace RomanticWeb
{
    public sealed class Triple
    {
        private readonly RdfNode _object;

        private readonly RdfNode _subject;

        private readonly RdfNode _predicate;

        public Triple(RdfNode s,RdfNode p,RdfNode o)
        {
            _object=o;
            _subject=s;
            _predicate=p;
        }

        public RdfNode Object
        {
            get
            {
                return _object;
            }
        }

        public RdfNode Predicate
        {
            get
            {
                return _predicate;
            }
        }

        public RdfNode Subject
        {
            get
            {
                return _subject;
            }
        }

        public static bool operator ==(Triple left, Triple right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Triple left, Triple right)
        {
            return !Equals(left, right);
        }

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