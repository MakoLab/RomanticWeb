using System;
using NullGuard;

namespace RomanticWeb.Entities
{
	public class EntityId
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
			this._uri=uri;
		}

        /// <summary>
        /// The underlying Uniform Resource Identifier
        /// </summary>
        public Uri Uri
        {
            get { return this._uri; }
        }

		/// <summary>
		/// Checks if two Entity identifiers are equal
		/// </summary>
		public static bool operator==([AllowNull] EntityId left,[AllowNull] EntityId right)
		{
			return Equals(left,right);
		}

		/// <summary>
		/// Checks if two Entity identifiers are not equal
		/// </summary>
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

        /// <summary>
        /// Gets the hash code
        /// </summary>
        public override int GetHashCode()
        {
            return this._uri.GetHashCode();
        }

        /// <summary>
        /// Checks if two Entity identifiers are equal
        /// </summary>
        public override bool Equals([AllowNull] object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return this._uri == ((EntityId)obj)._uri;
        }

        public override string ToString()
        {
            return this.Uri.ToString();
        }

        /// <summary>
        /// Check for equality with <param name="other"></param>
        /// </summary>
        protected bool Equals([AllowNull] EntityId other)
        {
            return other != null && Equals(this._uri, other._uri);
        }
	}
}