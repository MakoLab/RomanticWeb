using System;
using NullGuard;

namespace RomanticWeb.Entities
{
    /// <summary>
    /// An Entity's identifier (URI or blank node)
    /// </summary>
	public class EntityId:IComparable,IComparable<EntityId>
    {
        private readonly Uri _uri;

		/// <summary>
		/// Creates a new instance of <see cref="EntityId"/> from string
		/// </summary>
		public EntityId(string uri):this(new Uri(uri))
		{
		}

		/// <summary>
		/// Creates a new instance of <see cref="EntityId"/> from an Uniform Resource Identifies
		/// </summary>
        public EntityId(Uri uri)
		{
			_uri=uri;
		}

        /// <summary>
        /// The underlying Uniform Resource Identifier
        /// </summary>
        public Uri Uri
        {
            get { return _uri; }
        }

#pragma warning disable 1591
		public static bool operator==([AllowNull] EntityId left,[AllowNull] EntityId right)
		{
			return Equals(left,right);
		}

		public static bool operator!=([AllowNull] EntityId left,[AllowNull] EntityId right)
		{
			return !(left==right);
		}

        public static implicit operator EntityId(string entityId)
        {
            EntityId result = null;
            if (entityId != null)
            {
                result = new EntityId(entityId);
            }

            return result;
        }
#pragma warning restore 1591

        /// <summary>
        /// Gets the hash code
        /// </summary>
        public override int GetHashCode()
        {
            return _uri.GetHashCode();
        }

        int IComparable.CompareTo(object obj)
        {
            return FluentCompare<EntityId>.Arguments(this,obj).By(id => id.Uri,new AbsoluteUriComparer()).End();
        }

        int IComparable<EntityId>.CompareTo(EntityId other)
        {
            return ((IComparable)this).CompareTo(other);
        }

        /// <summary>
        /// Checks if two Entity identifiers are equal
        /// </summary>
        public override bool Equals([AllowNull] object obj)
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

        /// <inheritdoc />
        public override string ToString()
        {
            return Uri.ToString();
        }

        /// <summary>
        /// Check for equality with <param name="other"></param>
        /// </summary>
        protected bool Equals([AllowNull] EntityId other)
        {
            return other != null && Equals(_uri, other._uri);
        }
    }
}