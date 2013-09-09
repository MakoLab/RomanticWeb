using System;

namespace RomanticWeb
{
    public class EntityId
    {
        protected bool Equals(EntityId other)
        {
            return Equals(_uri, other._uri);
        }

        public override int GetHashCode()
        {
            return (_uri != null ? _uri.GetHashCode() : 0);
        }

        private readonly Uri _uri;

        public EntityId(string uri) : this(new Uri(uri))
        {
        }

        public EntityId(Uri uri)
        {
            _uri = uri;
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

            return _uri == ((EntityId)obj)._uri;
        }

        public static bool operator ==(EntityId left, EntityId right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EntityId left, EntityId right)
        {
            return !(left == right);
        }

        public Uri Uri
        {
            get { return _uri; }
        }
    }
}