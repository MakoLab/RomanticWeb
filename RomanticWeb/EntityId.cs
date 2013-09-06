using System;

namespace RomanticWeb
{
    public class EntityId
    {
        protected bool Equals(EntityId other)
        {
            return Equals(_iri, other._iri);
        }

        public override int GetHashCode()
        {
            return (_iri != null ? _iri.GetHashCode() : 0);
        }

        private readonly Uri _iri;

        public EntityId(string iri)
        {
            _iri = new Uri(iri);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return _iri == ((EntityId)obj)._iri;
        }

        public static bool operator ==(EntityId left, EntityId right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EntityId left, EntityId right)
        {
            return !(left == right);
        }
    }
}